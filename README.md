# Small Scheduling App

FYI, this project is a very small app that was mostly generated with claude with minimal developer oversight. It will be pretty garbage most likely and if anyone is using it for anything meaningful, it should be created with more care. You have been warneed.

[![Build and Release](https://github.com/YolBolsun/SmallSchedulingApp/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/YolBolsun/SmallSchedulingApp/actions/workflows/build-and-release.yml)

A Windows taskbar calendar application built with WinUI 3.

## Download & Install

**For Users:** Download the latest release from the [Releases page](https://github.com/YolBolsun/SmallSchedulingApp/releases).

### Quick Installation:
1. Download all files from the latest release
2. Right-click `Install.ps1` and select "Run with PowerShell"
3. Allow it to run as Administrator when prompted
4. Find SmallSchedulingApp in your Start Menu

See [INSTALLATION.md](INSTALLATION.md) in the release for detailed instructions and alternative installation methods.

---

## For Developers

## Requirements

- Visual Studio 2022 (version 17.0 or later)
- Windows 11 or Windows 10 version 1809 (build 17763) or later
- .NET 8.0 SDK
- **Windows App SDK workload** for Visual Studio 2022

## How to Install Required Visual Studio Workloads

The "Class not registered" error typically indicates missing Visual Studio workloads.

1. Open **Visual Studio Installer**
2. Click **Modify** on your Visual Studio 2022 installation
3. Ensure these workloads are installed:
   - **.NET Desktop Development**
   - **Universal Windows Platform development**
   - Under "Individual Components", search for and install:
     - **Windows App SDK C# Templates**
     - **Windows 11 SDK (10.0.22621.0 or later)**

4. Click **Modify** to install the selected components

## Running the Application

1. Open `SmallSchedulingApp.sln` in Visual Studio 2022
2. Ensure the build configuration is set to **Debug** and platform is set to **x64**
3. Press **F5** or click the **Start** button to build and run the application

**Note**: Do NOT run the .exe file directly from the bin folder. WinUI 3 packaged apps must be deployed through Visual Studio's debugging process, which handles the necessary app registration.

## Building from Command Line

```bash
# This will build but may not deploy properly for running
dotnet build SmallSchedulingApp.sln -c Debug
```

For proper deployment and running, use Visual Studio 2022.

## Troubleshooting

### "Class not registered" Error
This error occurs when trying to run the app without proper deployment. Solutions:
- Always run from Visual Studio (F5) during development
- Ensure Windows App SDK workload is installed in Visual Studio
- Don't run the .exe directly from the bin folder

### Build Errors Related to MSIX or Packaging
- Ensure you have the Windows App SDK workload installed
- Make sure you're using Visual Studio 2022 (not just VS Code or command line)

## Project Structure

- `SmallSchedulingApp/` - Main application project
  - `App.xaml` / `App.xaml.cs` - Application entry point
  - `MainWindow.xaml` / `MainWindow.xaml.cs` - Main window UI
  - `Package.appxmanifest` - Windows app package manifest
  - `Assets/` - Application icons and assets

## Creating a Release

See [RELEASE.md](RELEASE.md) for instructions on creating and publishing new releases.

**Quick Release:**
```bash
git tag v1.0.0
git push origin v1.0.0
```

GitHub Actions will automatically build and publish the release!
