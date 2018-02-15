# smrtapi.net
C# client for Speechmatics real time API

## Installation
```powershell
Install-Package Speechmatics.Realtime.Client
```

## Sample code
```csharp
namespace DemoApp
{
    public class Program
    {
        private const string SampleAudio = "2013-8-british-soccer-football-commentary-alex-warner.mp3";

        // ReSharper disable once UnusedParameter.Local
        public static void Main(string[] args)
        {
            var builder = new StringBuilder();

            using (var stream = File.Open(SampleAudio, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    /*
                     * The API constructor is passed the websockets URL, callbacks for the messages it might receive,
                     * the language to transcribe and stream to read data from.
                     */
                    var api = new SmRtApi("wss://api.rt.speechmatics.io:9000/",
                        s => builder.Append(s),
                        stream,
                        new SmRtApiConfig("en-US")
                    );
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
    }
}
```

# Sample Dockerfile

```
FROM microsoft/dotnet as build-env
WORKDIR /app

RUN git clone https://github.com/jrg1381/smrtapi.net.git
WORKDIR /app/smrtapi.net/SmRtAPI/DemoAppNetCore
RUN sed -i 's/<SignAssembly>true<\/SignAssembly>/<SignAssembly>false<\/SignAssembly>/' ../SmRtAPI/SpeechmaticsAPI.csproj
RUN dotnet build && dotnet publish -c Release -o out
RUN cp ../DemoApp/*.mp3 ./out

FROM microsoft/dotnet:runtime
WORKDIR /app
COPY --from=build-env /app/smrtapi.net/SmRtAPI/DemoAppNetCore/out ./
ENTRYPOINT ["dotnet", "DemoAppNetCore.dll"]
```
