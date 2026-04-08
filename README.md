# CS 1.6 External Cheat

External ESP + Aimbot for Counter-Strike 1.6 (Build 8684 | Others will be implemented later)

### IMPORTANT!
- This version doesn't work on public servers, because I didn't find players list offset

## Features

### ESP
- Player boxes with team colors
- Health bars with dynamic colors
- Distance-based box scaling
- Snaplines (optional)

### Aimbot (Mouse button 4 to active)
- Smooth aiming (not implemented yet)
- Target selection (closest to crosshair[not implemented yet] / lowest distance)
- Team check

### Overlay (Home button to switch on/off)
- ImGui-based menu
- Toggle features on/off

## Supported Game Version

- CS 1.6 Build 8684 (Non-Steam / Steam)
- OpenGL mode (hw.dll)

## Offsets

| Offset | Module | Description |
|--------|--------|-------------|
| `0xEC9780` | hw.dll | View/Projection matrix (column-major, 16 floats) |
| `0x7F6304` | hw.dll | Player entity list pointer |
| `0x7C` | hw.dll | Local player base address |
| `0x108AEC4` | hw.dll | Local dwViewAngles |
| `0x8` | hw.dll+entityBaseAddress+0x04 | Entity position |
| `0x160` | hw.dll+entityBaseAddress+0x04 | Entity health |
| `0x324` | hw.dll | Step between entities |

## Keybinds

| Key | Action |
|-----|--------|
| INSERT | Toggle menu |
| Home | Toggle Overlay |
| Side mouse button | Active Aimbot |

## Build Requirements

- .NET 6.0 or higher
- ImGui.NET
- Native Windows API

## Usage

1. Launch Counter-Strike 1.6
2. Run CS-1.6-Cheat.exe
3. Enjoy!

## Important Notes

- Read-and-Write memory operations
- External cheat = no DLL injection
- Tested only on OpenGL API

## Disclaimer

This project is for **educational purposes only**. Use at your own risk.