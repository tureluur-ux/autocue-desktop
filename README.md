# AutoCue

AutoCue is a lightweight Windows teleprompter for rehearsals, presentations, and recording. Write or open a `.txt`/`.md` script, set a comfortable reading speed and text size, then start the smooth, hands-free scroll.

## Features

- Create scripts in the built-in editor, or open `.txt` and `.md` files from the file picker or by drag and drop.
- Preview your script, make edits, and save it as a UTF-8 `.txt` or `.md` file.
- Smooth scrolling with an adjustable speed from 10 to 240 pixels per second.
- Adjustable, high-contrast script text from 24 to 56 pixels.
- Start or pause with the on-screen button or the **Space** key; save changes with **Ctrl+S**.
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

For a direct download, open [the latest AutoCue release](https://github.com/tureluur-ux/autocue-desktop/releases/latest) and download `AutoCue.exe` under **Assets**.

## Preferences

AutoCue stores its settings locally in `%LOCALAPPDATA%\AutoCue\settings.json`. Your script contents are only read locally and are not copied or uploaded.
