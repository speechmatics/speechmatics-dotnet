# smrtapi.net
C# client for Speechmatics real time API

## Installation
```powershell
Install-Package Speechmatics
```

## Sample code
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Speechmatics.Realtime.Client;
using Newtonsoft.Json;
using Speechmatics.Realtime.Client.Config;

namespace DemoApp
{
    public class Program
    {
        private const string SampleAudio = "2013-8-british-soccer-football-commentary-alex-warner.mp3";

        private static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        private static string RtUrl
        {
            get
            {
                return "wss://staging.realtimeappliance.speechmatics.io:9000/v2";
                var host = Environment.GetEnvironmentVariable("TEST_HOST") ?? "api.rt.speechmatics.io";
                return host.StartsWith("wss://") ? host : $"wss://{host}:9000/";
            }
        }

        public static void Main(string[] args)
        {
            var start = DateTime.Now;
            Debug.WriteLine("Starting at {0}", start);
            var builder = new StringBuilder();
            var language = Environment.GetEnvironmentVariable("LANG") ?? "en";
            Console.WriteLine(language);

            using (var stream = File.Open(SampleAudio, FileMode.Open, FileAccess.Read))
            {
                try
                {

                    var config = new SmRtApiConfig(language)
                    {
                        AuthToken= Environment.GetEnvironmentVariable("AUTH_TOKEN"),
                        // GenerateTempToken = True <- set this to True for accounts from portal.speechmatics.com
                        OutputLocale = "en-GB",
                        AddTranscriptCallback = s => builder.Append(s),
                        AddTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddTranslationMessageCallback = s => Console.WriteLine(ToJson(s)),
                        AddPartialTranscriptMessageCallback = s => Console.WriteLine(ToJson(s)),
                        ErrorMessageCallback = s => Console.WriteLine(ToJson(s)),
                        WarningMessageCallback = s => Console.WriteLine(ToJson(s)),
                        CustomDictionaryPlainWords = new[] {"speechmagic"},
                        CustomDictionarySoundsLikes = new Dictionary<string, IEnumerable<string>>(),
                        Insecure = true,
                        EnablePartials=true,
                        TranslationConfig = new TranslationConfig() {
                            TargetLanguages = new [] {"de"},
                            EnablePartials = true
                        }
                    };

                    // We can do this here, or earlier. It's not used until .Run() is called on the API object.
                    config.CustomDictionarySoundsLikes["gnocchi"] = new[] {"nokey", "noki"};

                    var api = new SmRtApi(RtUrl,
                        stream,
                        config
                    );
                    // Run() will block until the transcription is complete.
                    Console.WriteLine($"Connecting to {RtUrl}");
                    api.Run();
                    Console.WriteLine(builder.ToString());
                }
                catch (AggregateException e)
                {
                    Console.WriteLine(e);
                }
            }

            var finish = DateTime.Now;
            Debug.WriteLine("Starting at {0} -- {1}", finish, finish-start);
            Console.ReadLine();
        }
    }
}
```

# Sample Dockerfile

```
FROM microsoft/dotnet as build-env
WORKDIR /app

RUN git clone https://github.com/speechmatics/speechmatics-dotnet.git
WORKDIR /app/smrtapi.net/SmRtAPI/DemoAppNetCore
RUN sed -i 's/<SignAssembly>true<\/SignAssembly>/<SignAssembly>false<\/SignAssembly>/' ../SmRtAPI/SpeechmaticsAPI.csproj
RUN dotnet build && dotnet publish -c Release -o out
RUN cp ../DemoApp/*.mp3 ./out

FROM microsoft/dotnet:runtime
WORKDIR /app
COPY --from=build-env /app/smrtapi.net/SmRtAPI/DemoAppNetCore/out ./
ENTRYPOINT ["dotnet", "DemoAppNetCore.dll"]
```

# Debugging

Merge the following with your `app.config` file to enable debugging to a file. If you don't have an `app.config` file, use this XML as-is. This technique will work whether you're building from source or using the Nuget package.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.diagnostics>
    <sources>
      <source name="System.Net" tracemode="includehex" maxdatasize="1024">
        <listeners>
          <add name="System.Net"/>
        </listeners>
      </source>
      <source name="System.Net.Cache">
        <listeners>
          <add name="System.Net"/>
        </listeners>
      </source>
      <source name="System.Net.Http">
        <listeners>
          <add name="System.Net"/>
        </listeners>
      </source>
      <source name="System.Net.Sockets">
        <listeners>
          <add name="System.Net"/>
        </listeners>
      </source>
      <source name="System.Net.WebSockets">
        <listeners>
          <add name="System.Net"/>
        </listeners>
      </source>
    </sources>
    <switches>
      <add name="System.Net" value="Verbose"/>
      <add name="System.Net.Cache" value="Verbose"/>
      <add name="System.Net.Http" value="Verbose"/>
      <add name="System.Net.Sockets" value="Verbose"/>
      <add name="System.Net.WebSockets" value="Verbose"/>
    </switches>
    <sharedListeners>
      <add name="System.Net"
           type="System.Diagnostics.TextWriterTraceListener"
           initializeData="network.log"
      />
    </sharedListeners>
    <trace autoflush="true">
      <listeners>
        <add name="file" type="System.Diagnostics.TextWriterTraceListener" initializeData="trace.log"/>
      </listeners>
    </trace>
    </system.diagnostics>
</configuration>
```

This will log smrtapi specific information to `trace.log` (and the output window in Visual Studio, if you're running under a debugger) and all network traffic to `network.log`.
