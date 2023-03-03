# unity-grpc

A gRPC-integrated Unity boilerplate with first-class tooling support for [Visual Studio Code](https://code.visualstudio.com/) and built-in support for NuGet packages. Stop paying for [Grpc for Unity](https://assetstore.unity.com/packages/tools/network/grpc-for-unity-216892).

## Requirements

- [.NET Core SDK](https://dotnet.microsoft.com/en-us/download)
- [.NET Desktop Build Tools](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)
- [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)

## Usage

See the following stripped-down example of a Unity client that connects to a gRPC server.

```cs
using Grpc.Core;
using UnityEngine;
using static RoadGeneratorService;

[DisallowMultipleComponent]
public class RoadGenerator : MonoBehaviour {
    GrpcChannel Channel { get; set; }
    RoadGeneratorServiceClient Client { get; set; }

    void OnEnable() {
        this.Channel = new GrpcChannel("localhost", 5000);
        this.Client = this.Channel.CreateClient<RoadGeneratorServiceClient>();

        GenerateRoadRequest generateRoadRequest = new() {
            RoadWidth = this.roadWidth,
            Difficulty = this.difficulty
        };

        ExceptionHandler.Handle<RpcException>("Server not found. Please start the server before entering play mode.", () => {
            _ = this.Client.Generate(generateRoadRequest);
        });
    }

    void Update() {
        ExceptionHandler.Handle<RpcException>("Server not found. Please start the server before entering play mode.", () => {
            GeneratedRoadResponse response = this.Client.Update(new GeneratedRoadRequest());

            this.GenerateRoadMesh(
                response.X.AsParallel(),
                response.Z.AsParallel()
            );
        }, out bool isError);

        if (isError) {
            // Handle error here
        }
    }

    void OnDisable() => this.Channel?.Dispose();
}
```

## Setup

Install and setup gRPC with the following.

```bash
dotnet publish UnityNuGet
```

You can enable Unity warnings for Visual Studio Code by running the following.

```bash
dotnet restore .vscode
```

## Development

### Swapping Runtimes

If you are using a different architecture than `win-x64`, you will need to change the `UnityBuildArchitecture` property in the [UnityNuGet.csproj](UnityNuGet/UnityNuGet.csproj) file.

```xml
<!-- valid runtimes: win-x64, win-x86, osx-x64, linux-x64, linux-arm64 -->
<UnityBuildArchitecture>win-x64</UnityBuildArchitecture>
```

### Add NuGet Packages

`UnityNuGet` is a native, fast and lightweight NuGet client wrapper for Unity. Powered by MSBuild and .NET CLI—say goodbye to the [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) bloatware!

```bash
dotnet add UnityNuGet package <package-name>
dotnet publish UnityNuGet
```

### Housekeeping

If you have removed many Unity packages and you are facing some issues in your editor, you may find it useful to remove all ignored files/directories.

```bash
git clean -fdX
```

## Known Issues

### Access Denied

If you are getting an error like the following, you will need to try again with the Unity process killed.

```text
error MSB3231: Unable to remove directory "../Assets/NuGet\". Access to the path 'grpc_csharp_ext.dll' is denied.
```
