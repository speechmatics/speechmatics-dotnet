# Installing .NET Core on Linux (Ubuntu)

There are several equivalent ways to install .NET Core on Ubuntu, either by using the Ubuntu distro repositories (which may have less current versions) or the official Microsoft repositories. This section documents one way of doing it.

## Prerequisites

1. Add the backports repository
```bash
sudo add-apt-repository ppa:dotnet/backports
```

2. Install the dotnet SDK
```bash
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-9.0
```

3. Verify Installation

Run the following command to verify the installation:

```bash
dotnet --version
```

You should see the installed version of .NET Core.

## Additional Resources

- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?tabs=dotnet9&pivots=os-linux-ubuntu-2204)

# Building

These instructions should also work when using .NET Core on other platforms.

First, build the library.

```bash
cd speechmatics-dotnet/SmRtAPI/SmRtAPI
dotnet restore
dotnet build
```

Then return to the root directory and build the demo app. You may need to edit the `.csproj` file to change the version of .NET Core being targetted. 

```bash
cd speechmatics-dotnet/SmRtAPI/DemoAppNetCore
dotnet restore
dotnet build
```

# Running

Check the connection with no credentials.

```bash
./bin/Debug/net9.0/DemoAppNetCore 
en
Connecting to wss://neu.rt.speechmatics.com/v2/en
{"reason":"Not Authorized","type":"not_authorised","seq_no":0,"duration_limit":0.0,"message":"Error"}
```

Credentials can be supplied by setting `SM_TOKEN` in the environment to your API key.

If not using the Speechmatics realtime SaaS, you can point the demo at a local docker container by setting the `RT_URL` environment variable to `ws://host:port/`

## Watching the transcript with `jq`

The default for the demo app is to print the complete JSON messages received to `stdout`. You can modify the code, or use `jq` to watch the text part of the transcripts as they come in:

```bash
export TEST_HOST=ws://localhost:9000
./bin/Debug/net9.0/DemoAppNetCore | grep '{' | jq '.|select(.message=="AddTranscript") | .metadata.transcript'
```

which will print something like

```
"Sees it, takes the "
"ball "
"and he's "
"down. "
"He's down. "
"Down. Brings "
"him down. "
"The referee's "
```

using the default input audio.