# smrtapi.net
C# client for Speechmatics real time API

## Installation
```powershell
Install-Package Speechmatics.Realtime.Client -Version 0.5.1
```

## Sample code
```csharp
public static void Main(string[] args)
{
    var builder = new StringBuilder();

    using (var stream = File.Open(SampleAudio, FileMode.Open, FileAccess.Read))
    {
        try
        {
            /*
             * The API constructor is passed the websockets URL, callbacks for 
             * the messages it might receive,the language to transcribe, and 
             * stream to read data from, assumed to be a media file such as
             * an mp3 or wav.
             *
             * A longer constructor creates an instance which can read raw 
             * PCM data from an audio source.
             */
             var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/",
                 s => builder.Append(s),
                 CultureInfo.GetCultureInfo("en-US"),
                 stream);
             // Run() will block until the transcription is complete.
             api.Run();
             Console.WriteLine(builder.ToString());
        }
        catch (AggregateException e)
        {
            Console.WriteLine(e);
        }
    }

    Console.ReadLine();
}
```
