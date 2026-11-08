# Developer-Console

A mod for The Long Dark that makes Hinterland's developer console accessible in-game.
Forked From: https://github.com/FINDarkside/TLD-Developer-Console

Works with The Long Dark **2.60** (Unity 6) and MelonLoader **0.7.3**.

## Instructions

- Press F1 to open or close the developer console
- Use `help` to get a list of all commands
- Use `search X` to get a list of all commands that include `X`
- Press the TAB key to auto-complete a command
- Auto-completion also works on the arguments of certain commands, such as `gear_add` and `scene`

Commands added by this mod on top of Hinterland's own:

| Command | What it does |
| --- | --- |
| `scene_name` | Prints the name of the active scene |
| `scene_list` | Lists all scenes known to the game (base game and installed DLC) |
| `parameter_list <command>` | Lists the auto-complete parameters registered for a command |
| `pos` | Prints the player position |
| `tp x z` or `tp x y z` | Teleports the player; with two coordinates the ground height is found automatically |
| `gear_list` | Lists all gear names |
| `gear_search <text>` | Lists gear names containing `text` |

## Installation

1. Install MelonLoader 0.7.3 or newer with the official installer from https://github.com/LavaGang/MelonLoader/releases/latest and point it at the game's `tld.exe`.
2. Launch the game once and wait until the main menu appears. On this first launch MelonLoader opens a console window and generates the game's interop assemblies into `MelonLoader/Il2CppAssemblies`, which takes a minute or two.
   - If the game starts normally but **no MelonLoader console window appears**, the `version.dll` proxy MelonLoader installed was not picked up. This happens when another overlay (Steam, NVIDIA App, ...) loads the real Windows `version.dll` into the game first. Rename `version.dll` in the game folder to `winhttp.dll` and launch again; MelonLoader supports both names.
3. Copy `DeveloperConsole.dll` (from this repository or the releases page) into the `Mods` folder in your The Long Dark install directory.

## Building

Requires a .NET SDK (6.0 or newer). The mod targets .NET 6 because that is what MelonLoader runs Il2Cpp mods on.

```
dotnet build DeveloperConsole/DeveloperConsole.csproj -c Release
```

By default this compiles against the `REDcat.Modding.TLD.Il2CppAssemblies` NuGet package for game version 2.60, so it works on CI and on a machine without the game.

To compile against the assemblies of your own installed game instead (always an exact match for the game and MelonLoader version you have), point `TLD_GAME_DIR` at the game folder before building, for example:

```
set TLD_GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\TheLongDark
dotnet build DeveloperConsole/DeveloperConsole.csproj -c Release
```

The output is `DeveloperConsole/bin/Release/net6.0/DeveloperConsole.dll`.

## Changelog

### 1.6.0
- Updated for The Long Dark 2.60 (Unity 6) and MelonLoader 0.7.x.
- The game removed the `BootUpdate` class this mod hooked to create the console, which made the mod fail to load since 2.5x. The console is now created from MelonLoader's scene callbacks once the first scene has loaded.
- Scene-name auto-completion for the `scene` command is refreshed as the game's content catalogs load, so DLC scenes are included.
- The build no longer depends on the discontinued `ds5678` assembly package; see Building.

### 1.5.2
- Fixed for game version 2.44.
