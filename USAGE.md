# Contents [#](#contents 'Go To Here')

- [ISmRtApi](#T-Speechmatics-Realtime-Client-Interfaces-ISmRtApi 'Speechmatics.Realtime.Client.Interfaces.ISmRtApi')
  - [CancelToken](#P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-CancelToken 'Speechmatics.Realtime.Client.Interfaces.ISmRtApi.CancelToken')
  - [WsUrl](#P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-WsUrl 'Speechmatics.Realtime.Client.Interfaces.ISmRtApi.WsUrl')
- [MessageWriter](#T-Speechmatics-Realtime-Client-MessageWriter 'Speechmatics.Realtime.Client.MessageWriter')
  - [SendData(data)](#M-Speechmatics-Realtime-Client-MessageWriter-SendData-System-ArraySegment{System-Byte}- 'Speechmatics.Realtime.Client.MessageWriter.SendData(System.ArraySegment{System.Byte})')
- [SmRtApi](#T-Speechmatics-Realtime-Client-SmRtApi 'Speechmatics.Realtime.Client.SmRtApi')
  - [#ctor(wsUrl,addTranscriptCallback,model,stream)](#M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream- 'Speechmatics.Realtime.Client.SmRtApi.#ctor(System.String,System.Action{System.String},System.Globalization.CultureInfo,System.IO.Stream)')
  - [#ctor(wsUrl,addTranscriptCallback,model,stream,audioFormat,audioFormatEncoding,sampleRate)](#M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream,Speechmatics-Realtime-Client-Enumerations-AudioFormatType,Speechmatics-Realtime-Client-Enumerations-AudioFormatEncoding,System-Int32- 'Speechmatics.Realtime.Client.SmRtApi.#ctor(System.String,System.Action{System.String},System.Globalization.CultureInfo,System.IO.Stream,Speechmatics.Realtime.Client.Enumerations.AudioFormatType,Speechmatics.Realtime.Client.Enumerations.AudioFormatEncoding,System.Int32)')
  - [AddTranscriptCallback](#P-Speechmatics-Realtime-Client-SmRtApi-AddTranscriptCallback 'Speechmatics.Realtime.Client.SmRtApi.AddTranscriptCallback')
  - [AudioFormat](#P-Speechmatics-Realtime-Client-SmRtApi-AudioFormat 'Speechmatics.Realtime.Client.SmRtApi.AudioFormat')
  - [AudioFormatEncoding](#P-Speechmatics-Realtime-Client-SmRtApi-AudioFormatEncoding 'Speechmatics.Realtime.Client.SmRtApi.AudioFormatEncoding')
  - [CancelToken](#P-Speechmatics-Realtime-Client-SmRtApi-CancelToken 'Speechmatics.Realtime.Client.SmRtApi.CancelToken')
  - [Model](#P-Speechmatics-Realtime-Client-SmRtApi-Model 'Speechmatics.Realtime.Client.SmRtApi.Model')
  - [SampleRate](#P-Speechmatics-Realtime-Client-SmRtApi-SampleRate 'Speechmatics.Realtime.Client.SmRtApi.SampleRate')
  - [WsUrl](#P-Speechmatics-Realtime-Client-SmRtApi-WsUrl 'Speechmatics.Realtime.Client.SmRtApi.WsUrl')
  - [Run()](#M-Speechmatics-Realtime-Client-SmRtApi-Run 'Speechmatics.Realtime.Client.SmRtApi.Run')

<a name='assembly'></a>
# Speechmatics.Realtime.Client [#](#assembly 'Go To Here') [=](#contents 'Back To Contents')

<a name='T-Speechmatics-Realtime-Client-Interfaces-ISmRtApi'></a>
## ISmRtApi [#](#T-Speechmatics-Realtime-Client-Interfaces-ISmRtApi 'Go To Here') [=](#contents 'Back To Contents')

##### Namespace

Speechmatics.Realtime.Client.Interfaces

<a name='P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-CancelToken'></a>
### CancelToken `property` [#](#P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-CancelToken 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Cancellation token for async operations

<a name='P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-WsUrl'></a>
### WsUrl `property` [#](#P-Speechmatics-Realtime-Client-Interfaces-ISmRtApi-WsUrl 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

The websocket URL this API instance is connected to

<a name='T-Speechmatics-Realtime-Client-MessageWriter'></a>
## MessageWriter [#](#T-Speechmatics-Realtime-Client-MessageWriter 'Go To Here') [=](#contents 'Back To Contents')

##### Namespace

Speechmatics.Realtime.Client

<a name='M-Speechmatics-Realtime-Client-MessageWriter-SendData-System-ArraySegment{System-Byte}-'></a>
### SendData(data) `method` [#](#M-Speechmatics-Realtime-Client-MessageWriter-SendData-System-ArraySegment{System-Byte}- 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Send a block of data as a sequence of AddData and binary messages.

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| data | [System.ArraySegment{System.Byte}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.ArraySegment 'System.ArraySegment{System.Byte}') |  |

<a name='T-Speechmatics-Realtime-Client-SmRtApi'></a>
## SmRtApi [#](#T-Speechmatics-Realtime-Client-SmRtApi 'Go To Here') [=](#contents 'Back To Contents')

##### Namespace

Speechmatics.Realtime.Client

##### Summary

Speechmatics realtime API. Each instance represents a separate connection to the transcription engine.

<a name='M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream-'></a>
### #ctor(wsUrl,addTranscriptCallback,model,stream) `constructor` [#](#M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream- 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

*Inherit from parent.*

##### Summary

Transcribe a file from a stream

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| wsUrl | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A websocket endpoint e.g. wss://192.168.1.10:9000/ |
| addTranscriptCallback | [System.Action{System.String}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.String}') | A callback function for the AddTranscript message |
| model | [System.Globalization.CultureInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Globalization.CultureInfo 'System.Globalization.CultureInfo') | Language model |
| stream | [System.IO.Stream](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.IO.Stream 'System.IO.Stream') | A stream to read input from |

<a name='M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream,Speechmatics-Realtime-Client-Enumerations-AudioFormatType,Speechmatics-Realtime-Client-Enumerations-AudioFormatEncoding,System-Int32-'></a>
### #ctor(wsUrl,addTranscriptCallback,model,stream,audioFormat,audioFormatEncoding,sampleRate) `constructor` [#](#M-Speechmatics-Realtime-Client-SmRtApi-#ctor-System-String,System-Action{System-String},System-Globalization-CultureInfo,System-IO-Stream,Speechmatics-Realtime-Client-Enumerations-AudioFormatType,Speechmatics-Realtime-Client-Enumerations-AudioFormatEncoding,System-Int32- 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Transcribe raw audio from a stream

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| wsUrl | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | A websocket endpoint e.g. wss://192.168.1.10:9000/ |
| addTranscriptCallback | [System.Action{System.String}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action{System.String}') | A callback function for the AddTranscript message |
| model | [System.Globalization.CultureInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Globalization.CultureInfo 'System.Globalization.CultureInfo') | Language model |
| stream | [System.IO.Stream](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.IO.Stream 'System.IO.Stream') | A stream to read input from |
| audioFormat | [Speechmatics.Realtime.Client.Enumerations.AudioFormatType](#T-Speechmatics-Realtime-Client-Enumerations-AudioFormatType 'Speechmatics.Realtime.Client.Enumerations.AudioFormatType') | Raw |
| audioFormatEncoding | [Speechmatics.Realtime.Client.Enumerations.AudioFormatEncoding](#T-Speechmatics-Realtime-Client-Enumerations-AudioFormatEncoding 'Speechmatics.Realtime.Client.Enumerations.AudioFormatEncoding') | PCM encoding type |
| sampleRate | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | e.g. 16000, 44100 |

<a name='P-Speechmatics-Realtime-Client-SmRtApi-AddTranscriptCallback'></a>
### AddTranscriptCallback `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-AddTranscriptCallback 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Callback executed when the AddTranscript message is received from the server.

<a name='P-Speechmatics-Realtime-Client-SmRtApi-AudioFormat'></a>
### AudioFormat `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-AudioFormat 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Enum of File or Raw

<a name='P-Speechmatics-Realtime-Client-SmRtApi-AudioFormatEncoding'></a>
### AudioFormatEncoding `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-AudioFormatEncoding 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

If AudioFormat is File, this must also be File. Otherwise, a choice of PCM encodings.

<a name='P-Speechmatics-Realtime-Client-SmRtApi-CancelToken'></a>
### CancelToken `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-CancelToken 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Cancellation token for async operations

<a name='P-Speechmatics-Realtime-Client-SmRtApi-Model'></a>
### Model `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-Model 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Transcription language as an ISO code, e.g. en-US, en-GB, fr, ru, ja...

<a name='P-Speechmatics-Realtime-Client-SmRtApi-SampleRate'></a>
### SampleRate `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-SampleRate 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Audio sample rate, e.g. 16000 (for 16kHz), 44100 (for 44.1kHz CD quality)

<a name='P-Speechmatics-Realtime-Client-SmRtApi-WsUrl'></a>
### WsUrl `property` [#](#P-Speechmatics-Realtime-Client-SmRtApi-WsUrl 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

The websocket URL this API instance is connected to

<a name='M-Speechmatics-Realtime-Client-SmRtApi-Run'></a>
### Run() `method` [#](#M-Speechmatics-Realtime-Client-SmRtApi-Run 'Go To Here') [=](#contents 'Back To Contents')

##### Summary

Start the message loop and do not return until the file is transcribed

##### Parameters

This method has no parameters.
