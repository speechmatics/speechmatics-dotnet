# Realtime API
This page specifies the Realtime API at its current state. The basic elements in the communication are the following:

* **Client** - An application connecting to the API, providing the audio and processing the transcripts received from the **Server**.
* **Server** (also called **API**) - An entry point of the API, allows external connections and provides the transcripts back.
* **Worker** - An internal speech recogniser. This is an internal entity that actually runs the heavy speech recognition.

This is a specification for Speechmatics Real-time API version 0.6.0.

## Client ↔ API endpoint

The communication is done using WebSockets, which are implemented in most of the modern web-browsers, as well as in many common programming languages (namely C++ and Python, for instance using http://autobahn.ws/).

### Messages
Each message that the **Server** accepts is a stringified JSON object with the following fields:

  * `message` - A name of the message we are sending. Any other fields depend on the value of the `message` and are described below.

The messages sent by the **Server** to a **Client** are stringified JSON objects as well.

The only exception is a binary message containing the audio.

The following values of the field `message` are supported:

#### StartRecognition ####
Initiates a recognition, based on details provided in the following fields:

Mandatory fields:

  * `message: "StartRecognition"`
  * `model` (String): language product used to process the job (for example `en-US`)
  * `audio_format` (Object:AudioType): audio stream type you the user is going to send: see [Supported audio types](#supported-audio-types).
  * `output_format` (Object:OutputFormat): Requested output format, see [Supported output formats](#supported-output-formats).

Optional fields:

  * `auth_token` (String): Client's API key. Required when authentication is used on the server side.
  * `user` (Int): An ID of the client. Required when authentication is used on the server side.
  * `config` (Object:RecognitionConfig): Set up configuration values for this recognition session, see [Available configuration values](#available-configuration-values).
  * `meta` (String): metadata about the job you would like to be able to view later, e.g., which of your sources this job is derived from (`-F meta=your_meta_tag`). Metadata is limited to 1024 characters per job. *Currently not implemented.*

A message `StartRecognition` has to be send exactly once after the WebSockets is open. Client must wait for a `RecognitionStarted` message before sending any audio.

In case of success, a message with the following fields is sent as a response:

  * `message: "RecognitionStarted"`
  * `id` (Int): A unique **job ID**, that is used to refer to this job in calls using other channels than this WebSocket, as well as in the standard API calls. On this socket, we always know what job are we dealing with, because the socket is persistent.

In case of failure, an [error message](#error-messages) is sent, with `type` being one of the following:
 `invalid_model, invalid_audio_type, not_authorised, insufficient_funds, not_allowed, job_error`

An example of the `StartRecognition` message:

```json
{
  "message": "StartRecognition",
  "auth_token": "your_API_token",
  "user": 1,
  "model": "en-US",
  "audio_format": {
    "type": "raw",
    "encoding": "pcm_f32le",
    "sample_rate": 16000
  },
  "output_format": {
    "type": "json"
  },
  "config": {
    "dynamic_transcript": {
      "enabled": true,
      "max_delay": 5.0,
      "min_context": 2.0
    }
  }
}
```

The example above starts a session with the American English recogniser ready to consume raw PCM encoded audio with float samples at 16kHz. It also enables the dynamic transcripts, setting their maximum delay to 5 seconds and minimum audio context of 2 seconds.

#### AddData ####
Adds more audio data to the recognition job started on this WebSocket using `StartRecognition`. The server will only accept audio after it initialised a job, which is indicated by a `RecognitionStarted` message. Only one audio stream in one format is currently supported per one WebSocket (and hence one recognition job). `AddData` requires the following fields:

  * `size` (Int): A number of bytes of audio data that will follow as a binary ArrayBuffer.
  * `offset` (Int): An offset in the raw data stream where this chunk of data starts. The first `AddData` must start at offset 0, any subsequent chunk of data must specify the offset calculated as the offset of the previously sent chunk + the size of the previously sent chunk + 1. *The value is currently ignored - it can be set to zero.*
  * `seq_no` (Int, optional): A sequence number, which should be unique. If any confirmation or an error message is sent that corresponds to this `AddData`, it will an additional field `seq_no` with this value, if it was specified.

Each `AddData` mesage has to be followed by a binary message containing the actual binary audio data as an ArrayBuffer type, containing `size` bytes of audio data.

In case of success, a DataAdded message is sent as a response. This message confirms that the **Server** has accepted the data and will make a corresponding **Worker** process it. If the **Client** implementation holds the data in it's internal buffer to resubmit in case of an error, it can safely discard the corresponding data after this message. The following fields are present in the response:

  * `message: "DataAdded"`
  * `offset` (Int): The value of the `offset` field in the corresponding `AddData` message
  * `size` (Int): The value of the `size` field in the corresponding `AddData` message
  * `seq_no` (Int, Optional): The value of the `seq_no` field in the corresponding `AddData` message, if it was provided.

Possible errors:

  * `data_error`, `job_error`, `buffer_error`

Any error message will contain an `offset` field to identify the problematic frame, and if `seq_no` was provided, it will be included as well.

##### Implementation details #####
Under special circumstances, such as when the client is sending the audio data faster than real time, the **Server** might read the data slower than the **Client** is sending it. The **Server** will not read the binary part of the `AddData` message if the `size` field is larger than the size of the internal buffer for the audio on the **Server**. Note that per each **Worker**, there is a separate buffer. In that case, the server will read any messages coming on the WebSocket, until enough space is made in the buffer by passing the data to a corresponding **Worker**. The **Client** will only receive the corresponding DataAdded response message once the binary data is read. The WebSocket might eventually fill all the TCP buffers on the way, causing a corresponding WebSocket to fail to write and close the connection [with prejudice](http://www.w3.org/TR/2011/WD-websockets-20110929/#concept-websocket-close-fail). The **Client** can use the [bufferedAmount](http://www.w3.org/TR/2011/WD-websockets-20110929/#dom-websocket-bufferedamount) attribute of the WebSocket to prevent this.

#### AddTranscript ####
This message is sent from the **Server** to the **Client**, when the **Worker** has provided the **Server** with a part of output. In current state, this happens when we reach an end of a sentence or a phrase. It contains the transcript of a part of audio the **Client** has send using `AddData` - the **final transcript**. It corresponds to the audio since the last `AddTranscript` message. The transcript is final - any further `AddTranscript` and `AddTranscript` messages will only correspond to the newly processed audio - what follows the audio this transcript represents. The format of the transcript is in the format the **Client** specified in the `output_format` field of `StartRecognition` message (*currently it's always text*). It is sent completely asynchronously with the incoming audio data. It contains the following fields:

  * `message: "AddTranscript"`
  * `start_time` (Number): The start time of the audio (in seconds) that the **transcript** covers. It does not have to correspond to the first entry in the transcript itself, in case there is simply no transcript for the initial part of the time this transcript covers - this time includes any silence and is not meant to be treated as a word alignment.
  * `length` (Number): The length of the audio (of seconds) that the **transcript** covers. It includes any optional silence as well - it corresponds to the length of the raw audio processed to get this transcript.
  * `transcript` (String/Object): The partial transcript of the audio, in a string format. For a JSON output format, the value will be the JSON object directly. For any other type, it will be a string. *Currently, it's always set to a string.*
    * String: a few examples to say it all - "Hi. ", "This is an example. "
  * `words` (An array of Word objects) [Added in v0.2.0] - contains all individual word in the `transcript` sorted by their time. A special `<sb>` word indicates a sentence boundary, which usually means a full stop in `transcript`. A `Word` object has the following properties:
    * `word` (String): A lowercase representation of a word. In the future, this will have the same case as the same word appearing in `transcript`.
    * `start_time` (Number): An approximate time of occurence (in seconds) of the audio corresponding to the beginning of the word.
    * `length` (Number) [Added in 0.5.0]: An approximate duration (in seconds) of the audio correpsonding to the word.

The `start_time` of a partial transcript is always the `start_time` of the previously sent partial transcript plus the `length` of the previous transcript.

#### AddPartialTranscript ####
A partial-transcript message. The structure is the same as `AddTranscript`. A partial transcript is a transcript that can be changed and expanded by a future `AddTranscript` or `AddPartialTranscript` message and corresponds to the part of audio since the last `AddTranscript` message.
*Currently all partial transcripts are lowercase and without any punctuation characters.*

#### AddDynamicTranscript ####
A dynamic transcript message - only used when recogniser_config.dynamic_transcript.enabled is set to `True` in the initial `StartRecognition` message.
The structure is the same as `AddTranscript`.
A dynamic transcript is a transcript gathered from partial transcripts (and eventually final transcripts),
with output frequency fine-tuned by user, being differnet to standard output frequency of partial transcripts.

When dynamic transcripts are used, both partial and final transcripts should be ignored, and each dynamic transcript
should be treated as a final one instead. Both partial and final transcripts are still sent for reference and additional information.

#### SetRecognitionConfig ####
Allows the **Client** to configure the recognition session even after the initial `StartRecognition` message. It is not
guaranteed that all the values a client can set in `StartRecognition` can be successfully changed during the recognition.
Refer to [Available configuration values](#available-configuration-values) for additional details.

  * `message: "SetRecognitionConfig"`
  * `config` (Object:RecognitionConfig): Set up configuration values for this recognition session, see [Available configuration values](available-configuration-values).

#### EndOfStream ####
This message is sent form the Client to the API to announce it has sent all the audio it intended to send. No more audio is accepted after this message (`AddData`). The Server will finish processing the audio it has received already and then send a EndOfTranscript message. This message is usually send at the end of file or when the microphone input is stopped.

  * `last_seq_no` the seq_no of the last `AddData` call we did

#### EndOfTranscript ####
Sent from the API to the Client when the API has finished all the audio, as marked with the `EndOfStream` message. The API sends this only after it sends all the corresponding `AddTranscript` messages first. Upon receiving this message, the Client can safely disconnect immediately, there will be no more messages coming from the API.

### Supported audio types
An `AudioType` object always has one mandatory field `type`, and potentially more mandatory field based on the value of `type`. The following types are supported:

**`type: "raw"`**

Raw audio samples, described by the following additional mandatory fields:

  * `encoding` (String): Encoding used to store individual audio samples. Currently supported values:
    * `pcm_f32le` - Corresponds to 32 bit float PCM used in the WAV audio format, little-endian architecture.
    * `pcm_s16le` - Corresponds to 16 bit signed integer PCM used in the WAV audio format, little-endian architecture.
  * `sample_rate` (Int): Sample rate of the audio

**`type: "opus"`**

A compressed codec, it's choice is not final yet, so no additional fields are specified yet. Not supported yet.

**`type: "file"`**

Any audio/video format supported by GStreamer. The `AddData` messages have to provide all the file contents, including any headers. The file is usually not accepted all at once, but segmented into reasonably sized messages.

Example `audio_format` field value:
  `audio_format: {type: "raw", encoding: "pcm_s16le", sample_rate: 44100}`

### Supported output formats
An `OutputFormat` object always has one mandatory field `type`, and potentially more mandatory field based on the value of `type`. The following types are supported:

**`type: "ttxt"`**

**`type: "json"`**

*Currently, the field is not used - all the transcript is always exactly in the format described in `AddTranscript` message.*

### Available configuration values
A `RecognitionConfig` specified various configuration values for the recognition engine. All the values are optional, using default values when not provided.

  * `dynamic_transcript` (Object:DynamicTranscriptConfig) - Configure dynamic transcripts. See [Dynamic transcripts](#dynamic-transcripts).
  * `additional_vocab` (List:String) [Added in 0.6.0]: A list of additional words to be added to the standard recognition vocabulary for this recognition session. A word can also be a phrase, as long as individual words in the phrase are separated by spaces. For example `["WebSockets", "gruntbuggly", "hurr durr"]`. Default is an empty list.

### Dynamic transcripts
A dynamic transcript is a transcript that doesn't use the default built-in mechanism to decide when to send which parts of transcript, but rather allows fine-tuning by the client.
It is useful to obtain configured live-captions, as it allows finer control over latency and duration, at the cost of reduced accuracy.
Dynamic transcripts are sent in an `AddDynamicTranscript` message.
It is configured by a `DynamicTranscriptConfig` in `dynamic_transcript` field of `RecognitionConfig` object in `config` field of `StartRecognition` message. By default it is disabled.

Note that a dynamic transcript is mostly obtained from the `words` field of partial transcripts. The same limitations that apply to a partial transcript apply to a dynamic transcript as well, such as the lack of punctuation.

A dynamic transcript can be changed during the recognition session via the `SetRecognitionConfig` message.

`DynamicTranscriptConfig` has the following mandatory fields:

  * `enabled` (Boolean): Whether AddDynamicTranscript messages should be sent.

`DynamicTranscriptConfig` has the following optional fields:

  * `max_chars` (Int): Maximum nuber of characters in a single dynamic transcript. Once enough transcript is collected to reach or exceed this limit, it is sent immediately. If the limit was exceeded, the last word is omitted from the output not to exceed this limit. The omitted part will be included in future messages as appropriate. If `max_chars` is set to 0, no character limit is used. 0 is the default value.
  * `min_context` (Float): Minimum length of audio context (in seconds) following a word to consider it fixed and include that word in a dynamic-transcript. If a word "dog" was recognised, it will only be considered for a dynamic transcript after another two seconds of audio context were provided. More context means it it less likely the recognised word will change, so more context increases the accuracy. If an event triggering a final transcript occurs, `min_context` is ignored and the whole remainder of the audio corresponding to the final transcript is immediately considered fixed. The shorter the context the worse the accuracy. Defaults to 2 seconds.
  * `max_delay` (Float): Maximum delay of dynamic transcripts with respect to the incoming audio stream. Note that this does not include the delay of the speech recognition itself. When `min_context` is used, the duration of audio corresponding to each dynamic transcript is going to be `max_delay`-`min_context`. When a final transcript overrides `min_context` requirement, the duration of asingle dynamic transcript can be up to `max_delay`. Default is 5s.

### Error messages
Error messages have the following fields:

  * `message: "Error"`
  * `code` (Int): A numerical code for the error. See below. TODO: This is not yet finalised.
  * `type` (String): A code for the error message. See the list of possible errors below.
  * `reason` (String): A human-readable reason for the error message.
  * `seq_no` (Int, optional): A `seq_no` of a corresponding API call, only present if the `seq_no` was specified in the API call.

TODO: numerical codes as well?

#### Error types ####

  * **`type: "invalid_message"`**
    * The message received was not understood.
  * **`type: "invalid_model"`**
    * Unable to use the model for the recognition. This can happen if the language is not supported at all, or is not available for the user.
  * **`type: "invalid_audio_type"`**
    * Audio type is not supported, is deprecated, or the audio_type is malformed.
  * **`type: "invalid_output_format"`**
    * Output format is not supported, is deprecated, or the output_format is malformed.
  * **`type: "not_authorised"`**
    * User was not recognised, or the API key provided is not valid.
  * **`type: "insufficient_funds"`**
    * User doesn't have enough credits or any other reason preventing the user to be charged for the job properly.
  * **`type: "not_allowed"`**
    * User is not allowed to use this message (is not allowed to perform the action the message would invoke).
  * **`type: "job_error"`**
    * Unable to do any work on this job, the **Worker** might have timed out etc.
  * **`type: "data_error"`**
    * Unable to accept the data specified - usually because there is too much data being sent at once
  * **`type: "buffer_error"`**
    * Unable to fit the data in a corresponding buffer. This can happen for clients sending the input data faster then real-time.
  * **`type: "protocol_error"`**
    * Message received vas syntactically correct, but could not be accepted due to protocol limitations. This is usually caused by messages sent in the wrong order.
  * **`type: "unknown_error"`**
    * An error that did not fit any of the types above.

Note that `invalid_message`, `protocol_error` and `uknown_error` can be triggered as a response to any type of messages.

The transcription is terminated and the connection is closed after any error.

### Warning messages
Warning messages have the following fields:

  * `message: "Warning"`
  * `code` (Int): A numerical code for the warning. See below. TODO: This is not yet finalised.
  * `type` (String): A code for the warning message. See the list of possible warnings below.
  * `reason` (String): A human-readable reason for the warning message.
  * `seq_no` (Int, optional): A `seq_no` of a corresponding API call, only present if the `seq_no` was specified in the API call.

#### Warning types ####

  * **`type: "duration_limit_exceeded"`**
    * The maximum allowed duration of a single utterance to process has been exceeded. Any AddData messages received that exceed this limit are confirmed with DataAdded, but are ignored by the transcription engine. Exceeding the limit triggers the same mechanism as receiving an `EndOfStream` message, so the Server will eventually send an `EndOfTranscript` message and suspend.
    * It has the following extra field:
      * `duration_limit` (Float): The limit that was exceeded (in seconds).

## Example communication
The communication consists of 3 stages - initialisation, transcription and a disconnect handshake.

On **initialisation**, the `StartRecognition` message is sent from the Client to the API and the Client should block and wait until it receives a `RecognitionStarted` message.

Afterwards, the **transcription** stage happens. The client keeps sending `AddData` messages. The API asynchronously replies with `DataAdded` messages. The API also asynchronously sends `AddPartialTranscript` and `Add Transcript` messages.

Once the client doesn't want to send any more audio, the **disconnect handshake** is performed. The Client sends an `EndOfStream` message as it's last message. No more messages are handled by the API afterwards. The API processes whatever audio it has buffered at that point and sends all the `AddTranscript` and `AddPartialTranscript` messages accordingly. Once the API processes all the buffered audio, it sends an `EndOfTranscript` message and the Client can then safely disconnect.

Note: In the example below, `->` denotes a message sent by the Client to the API, `<-` denotes a message send by the API to the Client. Any comments are denoted `"[like this]"`.
```json
->  {"message":"StartRecognition","auth_token":"your_API_token","user":1,"model":"en-US",
    "audio_format":{"type":"raw","encoding":"pcm_f32le","sample_rate":16000},
    "output_format":{"type":"json"}
    }

 <- {"message": "RecognitionStarted", "id": 1}

->  {"message":"AddData","size":5460,"offset":0,"seq_no":0}

->  "[binary message - data in PCM 32b floats, little-endian; 5460 bytes]"

 <- {"offset": 0, "seq_no": 0, "message": "DataAdded", "size": 5460}

->  {"message":"AddData","size":5460,"offset":0,"seq_no":1}

->  "[binary message - data]"

 <- {"offset": 0, "seq_no": 0, "message": "DataAdded", "size": 5460}

"[more AddData messages with their DataAdded messages arriving asynchronously]"

"[asynchronously received transcripts:]"

 <- {"message": "AddPartialTranscript", "length": 0.234384956, "transcript": "one", "start_time": 0.0}

 <- {"message": "AddPartialTranscript", "length": 0.496936678, "transcript": "one to", "start_time": 0.0}

 <- {"message": "AddPartialTranscript", "length": 0.723905202, "transcript": "one to three", "start_time": 0.0}

 <- {"message": "AddTranscript", "length": 0.983209543, "transcript": "One two three. ", "start_time": 0.0}

"[closing handshake after some more data:]"

->  {"message":"EndOfStream","last_seq_no":10}

 <- {"message": "AddTranscript", "start_time": 0.938437504, "transcript": "some words", "length": 1.184575008}

 <- {"message": "EndOfTranscript"}
```

![A visual representation of messages corresponding to the example](./src/images/api_messages_diagram.png)
