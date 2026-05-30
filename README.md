# Network Monitor

A lightweight, real-time Windows network monitor that lives in the corner of your screen. It displays live download and upload statistics — current speed, session totals, all-time totals, and running averages — with a minimal borderless UI designed to stay out of your way.

Originally written entirely by hand in VB.NET in 2015, before AI tooling existed. Refactored in 2025 for correctness, thread safety, and efficiency while keeping all original behaviour intact.

---

## Features

- **Live stats** updated every second: current speed, session total, all-time total, and running average — for both download and upload
- **Borderless, always-on-top** window that snaps to screen edges and works correctly across multiple monitors
- **Adjustable opacity** when the window loses focus (25 / 50 / 75 / 100%)
- **Horizontal resize** by dragging the left or right edge
- **Right-click menu** to switch network adapter, toggle always-on-top, change opacity, or exit
- **Text-to-speech readout** of any stat via global hotkeys (works even when the app is not focused)
- **Session reset** via hotkey or adapter switch
- **Registry persistence** — window size, position, adapter choice, and opacity survive restarts
- **Live graph** (powered by [Graphy](https://github.com/Tayx94/graphy)) showing download and upload history
- **Error logging** to `errorLog.txt` alongside the executable

---

## Requirements

- Windows 10 or 11
- .NET Framework 4.7.1 (included in Windows 10 1703+ and all Windows 11 builds)
- A network adapter visible to `System.Net.NetworkInformation`

---

## Building

This project targets .NET Framework 4.7.1 and must be built with MSBuild (not the `dotnet` CLI).

```
msbuild NetworkMonitor\NetworkMonitor.vbproj /p:Configuration=Release
```

Or open `NetworkMonitor.sln` in Visual Studio 2017 or later and build normally.

The `Graphy.dll` dependency is a pre-built binary included in the repository under `bin\Debug` and `bin\Release`. No package restore is needed.

---

## Usage

Run `NetworkMonitor.exe`. The window appears in the bottom-right corner of your primary screen.

### Moving and resizing

- **Drag** anywhere on the window to move it. It snaps to screen edges automatically; drag past the snap zone to release.
- **Drag the left or right edge** to resize the window horizontally.
- The window remembers its position and size between sessions.

### Right-click menu

| Option | Description |
|---|---|
| Network Adapters | Select which adapter to monitor; switching resets the session |
| Always On Top | Toggle whether the window floats above other windows |
| Unfocused Opacity | Set how transparent the window becomes when not hovered |
| Exit | Close the application |

You can also press **Escape** to exit.

### Global hotkeys

These work system-wide, even when the window is minimised or behind other windows. The current value of each stat is read aloud via text-to-speech.

| Hotkey | Reads aloud |
|---|---|
| Ctrl + F1 | Total download (all-time) |
| Ctrl + F2 | Session download |
| Ctrl + F3 | Current download speed |
| Ctrl + F4 | Average download speed |
| Ctrl + F5 | Total upload (all-time) |
| Ctrl + F6 | Session upload |
| Ctrl + F7 | Current upload speed |
| Ctrl + F8 | Average upload speed |
| Ctrl + F12 | Reset session stats |

> If a hotkey conflicts with another application, the TTS for that key will silently do nothing. The hotkeys are registered at startup and released on exit.

### Tooltip

Hovering over any **Total** label shows a combined download + upload tooltip.

### Opacity behaviour

The window becomes semi-transparent when your cursor leaves it, using the opacity level you selected. It returns to full opacity when you hover over it. While dragging or resizing, opacity stays at 100%.

---

## Project structure

| File | Purpose |
|---|---|
| `MainForm.vb` | UI, event handling, window behaviour, background loop |
| `Network.vb` | Adapter enumeration and byte-count reads |
| `Download.vb` / `Upload.vb` | Shared stat storage (Current, Total, Session, Average) |
| `StatsSync.vb` | Shared lock object for cross-thread stat access |
| `RegistryEditor.vb` | Registry read/write for persistence |
| `Logger.vb` | Appends exceptions to `errorLog.txt` |
| `CustomKeys.vb` | Key press/release state tracker (up/down/pressed) |

---

## Notes

- The all-time **Total** stat reflects the cumulative bytes reported by the adapter since Windows last reset its counters (typically since the adapter was last reconnected or the system rebooted). It is not a lifetime counter stored by this app.
- The **Average** is a session average: total session bytes divided by elapsed seconds. It becomes less responsive over long sessions by design.
- If no matching adapter is found on startup, the display shows zeros until an adapter becomes available. The adapter list is re-checked every 10 seconds in the background.
