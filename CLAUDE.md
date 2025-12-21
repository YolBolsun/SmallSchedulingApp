# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SmallSchedulingApp is a Windows taskbar calendar application that provides event management with recurring events. The application sits in the Windows taskbar beside the default calendar.

## Core Architecture Concepts

### Calendar View
- Displays current month with next/previous month navigation buttons
- Similar look and feel to default Windows calendar
- Dates with events are circled
- Hovering over dates with events expands to show event list
- Each event in hover view has a red X for deletion (prompts for single occurrence vs all occurrences)

### Event System
Events support recurring patterns with these fields:
- **Event Name**: Text identifier
- **Event Start Date**: Initial occurrence date
- **Frequency**: Daily, Weekly, Bi-weekly, Monthly (default: Daily)
- **Event Occurrences**: Number of times to repeat (default: 1 for one-time event)

### Main User Flows

1. **Add Event**: Button opens form to create new events with name, start date, frequency, and occurrence count
2. **Explore Event**: Button present but non-functional (placeholder for future feature)

## Technology Stack

- **Framework**: WinUI 3 (Windows App SDK 1.6+)
- **Language**: C# with .NET 9.0
- **Target Platform**: Windows 10/11 (minimum version 10.0.17763.0)
- **Project Type**: Packaged Windows application (MSIX)

## Development Requirements

- **Visual Studio 2022** (version 17.0 or later) with:
  - .NET Desktop Development workload
  - Universal Windows Platform development workload
  - Windows App SDK C# Templates (Individual Components)
  - Windows 11 SDK (10.0.22621.0 or later)

## Development Commands

### Build the project
```bash
dotnet build SmallSchedulingApp.sln -c Debug
```

### Clean build artifacts
```bash
dotnet clean SmallSchedulingApp.sln
```

### Run the application
**IMPORTANT**: WinUI 3 packaged apps must be run through Visual Studio for proper deployment:
1. Open `SmallSchedulingApp.sln` in Visual Studio 2022
2. Set configuration to **Debug** and platform to **x64**
3. Press **F5** to build, deploy, and run

**Do NOT** run the .exe directly from the bin folder - this will cause "Class not registered" errors because the app requires proper MSIX deployment which Visual Studio handles automatically during F5 debugging.

## Project Structure

- `SmallSchedulingApp/` - Main application project
  - `App.xaml` / `App.xaml.cs` - Application entry point and lifecycle
  - `MainWindow.xaml` / `MainWindow.xaml.cs` - Main window UI and logic
  - `Package.appxmanifest` - Windows app package manifest
  - `Assets/` - Application icons and assets (see Assets/README.txt for required images)
