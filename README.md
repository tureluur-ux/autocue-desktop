# AutoCue

AutoCue is a lightweight Windows teleprompter for rehearsals, presentations, and recording. Open a `.txt` or `.md` script, set a comfortable reading speed and text size, then start the smooth, hands-free scroll.

## Features

- Open text and Markdown files from the file picker or drag and drop them onto the app.
- Smooth scrolling with an adjustable speed from 10 to 240 pixels per second.
- Adjustable, high-contrast script text from 24 to 56 pixels.
- Start or pause with the on-screen button or the **Space** key.
- Speed and text-size preferences are saved automatically for the next launch.
- Native Windows interface with no account, network access, or script upload.

## Run from source

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) on Windows, then run:

```powershell
dotnet run --project .\AutoCue.csproj
```

Run the focused settings tests with:

```powershell
dotnet test .\AutoCue.Tests\AutoCue.Tests.csproj
```

## Build a standalone Windows executable

From the repository root in PowerShell:

```powershell
.\scripts\publish-windows.ps1
```

The script publishes a self-contained, single-file x64 Windows app to `artifacts\windows-x64\AutoCue.exe`. It includes the .NET runtime, so the target PC does not need .NET installed. To run it, double-click `AutoCue.exe`.

The equivalent command is:

```powershell
dotnet publish .\AutoCue.csproj --configuration Release --runtime win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true -o .\artifacts\windows-x64
```

Each Windows build and pull request also runs the tests and publishes the executable as a downloadable `AutoCue-win-x64` workflow artifact.

## Preferences

AutoCue stores its settings locally in `%LOCALAPPDATA%\AutoCue\settings.json`. Your script contents are only read locally and are not copied or uploaded.
