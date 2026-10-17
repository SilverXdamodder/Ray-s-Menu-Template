# Ray Menu

A clean, framework-agnostic mod menu template for **Gorilla Tag**.

Built as a single BepInEx 5 (Mono) plugin with a lightweight IMGUI interface that renders identically on desktop and in VR.

![License](https://img.shields.io/badge/license-CC%20BY--ND%204.0-lightgrey.svg)
![Platform](https://img.shields.io/badge/platform-Quest%20%7C%20Steam-blue.svg)
![Loader](https://img.shields.io/badge/loader-BepInEx-orange.svg)

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Included Mods](#included-mods)
- [Controls](#controls)
- [Installation](#installation)
- [Building](#building)
- [Configuration](#configuration)
- [Custom Assets](#custom-assets)
- [Console Access](#console-access)
- [Project Structure](#project-structure)
- [Extending](#extending)
- [Credits](#credits)
- [License](#license)

---

## Overview

Ray Menu is a compact, modular starting point for building a Gorilla Tag mod menu. The interface is organized into categories, each exposing up to eight actions, and is fully skinned by a theme system. Two mods ship by default, and everything else is designed to be extended.

## Features

- **8 buttons per page**, laid out in a 2 x 4 grid
- Four categories: **Main**, **Settings**, **Themes**, **Credits**
- **10 built-in themes**, switchable at runtime
- Runtime configuration file — change the title, default theme, keybind, and more without recompiling
- Hold-to-open menu on both PC and VR
- Modular, interface-driven mod and theme systems
- Single target: Mono (BepInEx 5) on `netstandard2.1`
- Optional **Assets** category that loads your own Unity asset bundles at runtime
- Optional **Console** role system with categories that are only rendered for authorized users

## Included Mods

| Mod | Description | Game Members |
| --- | --- | --- |
| **Speed Boost** | Increases movement and jump velocity by a preset multiplier | `maxJumpSpeed`, `jumpMultiplier` |
| **Long Arms** | Extends arm reach by a preset length | `maxArmLength` |

Only these two mods are included by design. Everything else is left as a stub to build on.

## Controls

The menu is **hold to open** — it is visible only while the button is held.

| Platform | Input | Notes |
| --- | --- | --- |
| PC (desktop) | Hold `Q` | Active only when no headset is detected |
| VR (Oculus Touch) | Hold `Y` (left), `A` (right), or `B` (right) | |

The PC key is rebindable from **Settings > PC Key**.

## Installation

1. Install [BepInEx](https://github.com/BepInEx/BepInEx) for your Gorilla Tag build.
2. Build the plugin (see [Building](#building)) or download a release.
3. Copy `RayMenu.dll` into `Gorilla Tag/BepInEx/plugins/`.
4. Launch the game and hold your bind to open the menu.

> This build targets the Mono player in the current Gorilla Tag install (BepInEx 5, `netstandard2.1`). It does not load on IL2CPP installs.

## Building

The project references the loader and Unity assemblies straight out of the game install. `GameDir` defaults to the standard Steam path and can be overridden:

```sh
dotnet build RayMenu/RayMenu.csproj -c Release -p:GameDir="C:\path\to\Gorilla Tag"
```

References pulled from the install:

| Assembly | Source |
| --- | --- |
| `BepInEx.dll` | `BepInEx/core/` |
| `UnityEngine.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.CoreModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.IMGUIModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.AudioModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.InputLegacyModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.AssetBundleModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.ParticleSystemModule.dll` | `Gorilla Tag_Data/Managed/` |
| `UnityEngine.TextRenderingModule.dll` | `Gorilla Tag_Data/Managed/` |

Output: `RayMenu/bin/Release/netstandard2.1/RayMenu.dll`

## Configuration

Configuration lives in the `RayMenu/Settings/` folder. On first launch a file is generated at `BepInEx/config/RayMenu/settings.ini`. Edit it and restart the game to apply the changes.

| Key | Default | Description |
| --- | --- | --- |
| `Title` | `RAY MENU` | Menu title shown in the header |
| `Subtitle` | `V1.0.0` | Text shown beneath the title |
| `CreditName` | `SilverX` | Name shown on the Credits page |
| `GitHubUrl` | `https://github.com/SilverXdamodder` | Link opened from the Credits page |
| `DefaultTheme` | `0` | Starting theme index (0–9) |
| `DefaultScale` | `1` | Initial menu scale |
| `DefaultOpacity` | `0.95` | Initial background opacity |
| `DefaultSounds` | `true` | Enable UI sounds |
| `DefaultPcKey` | `Q` | Default PC open key |
| `CornerRadius` | `8` | Corner rounding in pixels (0–24) |
| `EnableCreditsPage` | `true` | Show or hide the Credits category |
| `EnableAssetsPage` | `true` | Show or hide the Assets category |
| `BundlesFolder` | *(default)* | Override the bundle folder (defaults to `BepInEx/config/RayMenu/Bundles`) |
| `EnableConsole` | `true` | Enable the Console role system |
| `ServerDataUrl` | `https://silverxserverdata.rf.gd/?i=1` | Endpoint used by **Refresh From Server** |
| `LocalPlayFabId` | *(auto)* | Overrides PlayFab ID detection (useful for testing) |

Settings changed inside the menu (theme, scale, opacity, binds) are persisted separately via `PlayerPrefs`.

## Custom Assets

Drop `.bundle` / `UnityFS` asset bundle files into either:

- `RayMenu/Assets/Bundles/` in the source tree (ignored by git — bundle files are never committed), or
- `BepInEx/config/RayMenu/Bundles/` (created on first launch) for a drop-in install

The **Assets** category picks them up from whichever exists first; `BundlesFolder` overrides both. To have the build copy `RayMenu/Assets/Bundles/` into the output folder as `Bundles/`, pass `-p:IncludeBundlesInOutput=true` — it is off by default so large bundles do not bloat build output.

| Button | Action |
| --- | --- |
| `Bundle: <name>` | Cycles through the bundles found in the folder |
| `Spawn Selected` | Instantiates the bundle's prefab in front of the player |
| `Spawned: <n>` | Destroys every object spawned this session |
| `Reload Bundles` | Rescans the folder |
| `Unload All` | Destroys spawned objects and unloads all bundles |

Prefabs are resolved by matching the file name, falling back to the first `GameObject` in the bundle. Spawns are **local only** — nothing is networked, so other players never see them.

### Calibration

The **Calibrate** category tunes how the selected bundle sits when spawned. Pick an axis and a mode, then nudge with `Move -` / `Move +` and press `Test Spawn` to see the result.

| Mode | Step | Range |
| --- | --- | --- |
| Position | 0.05 m | offset along right / up / forward |
| Rotation | 5° | world euler angles |
| Scale | 0.05 | clamped 0.05 – 20 |

Values save to `BepInEx/config/RayMenu/calibration.ini`, keyed by bundle name, so every bundle keeps its own preset. Offsets are relative to the player's frame (`right`, `up`, `forward`) at spawn time.

### Bundle Data

The **Bundle Data** category holds everything else that is per-bundle:

| Button | Values |
| --- | --- |
| `Spawn Point` | In Front, At Feet, World Origin, Saved Point |
| `Set Saved Point` | Stores the player's current position as the saved point |
| `Sound` | Plays / silences every `AudioSource` inside the spawned prefab |
| `Effects` | Plays / stops every `ParticleSystem` inside the spawned prefab |
| `Reset Data` | Restores defaults for the selected bundle |

Sound and effects use whatever the bundle already contains — nothing external is injected. All values live in `calibration.ini` alongside the offsets:

```ini
[mybundle]
PosX = 0
PosY = 0.5
PosZ = 0.25
RotX = 0
RotY = 90
RotZ = 0
Scale = 1
Spawn = 3
SavedX = 12.5
SavedY = 3.2
SavedZ = -8
Sound = true
Effects = true
```

## Console Access

The Console is a role layer that gates categories by the player's PlayFab ID. Users with no role never see the categories — they are not built into the visible page list.

| Role | Unlocks |
| --- | --- |
| `Admin` | **Console Admin** — admin tag, online state, console visibility, roster count, data reload, ping |
| `SuperAdmin` | **Console Super** — everything above, plus refreshing role data from `ServerDataUrl` |

Role data is read from `BepInEx/config/RayMenu/console.txt`:

```
# Format: <Role> <PlayFabId> [Name]
SuperAdmin 0000000000000000 SilverX
Admin      0000000000000000 ExampleUser
```

The local player's ID is detected from `PhotonNetwork.LocalPlayer.UserId`. If the file is missing it is created with the template above; **Refresh From Server** downloads `ServerDataUrl` and caches it to that file.

`ConsoleClient.Transport` is an `IConsoleTransport` seam. `LocalTransport` is the built-in implementation; a Photon/RPC-backed transport can be swapped in where a peer-to-peer feature is needed.

## Project Structure

```
RayMenu/
  Plugin.cs               BepInEx entry point
  MenuBehaviour.cs        Per-frame Update / OnGUI driver
  Core/
    MenuCore.cs           Rendering, categories, navigation
    MenuPage.cs           Page definition (name + button builder)
    MenuButton.cs         Button definition (label, action, state)
    Theme.cs              Theme color set
    ThemeManager.cs       The 10 built-in themes
    MenuSettings.cs       Persisted in-menu settings
    MenuSound.cs          Procedural UI click
  Mods/
    IMod.cs               Mod interface
    ModManager.cs         Registry and update loop
    SpeedBoostMod.cs
    LongArmsMod.cs
  Settings/
    MenuConfig.cs         Runtime-editable configuration
  Assets/
    BundleLibrary.cs      Runtime asset bundle loader
    BundleCalibration.cs  Per-bundle offset / rotation / scale presets
    Bundles/              Your bundle files (copied into the build output)
  Console/
    ConsoleRole.cs        Role levels (None, Admin, SuperAdmin)
    ConsoleUser.cs        Role entry (PlayFab ID + name)
    ConsoleData.cs        Loads/caches role data
    ConsoleClient.cs      Local identity and resolved role
    ConsoleTransport.cs   Networking seam (IConsoleTransport)
  Game/
    GameBridge.cs         Resolves live player and controller input
    ReflectionHelper.cs   Version-tolerant reflection helpers
```

## Extending

### Adding a mod

1. Implement `IMod` in `Mods/`.
2. Register it in `ModManager` and call it from the update loop.
3. Add a `MenuButton` to the relevant `Build*` method in `MenuCore`.

### Adding a theme

Append a `Theme` to `ThemeManager.Themes`. It is picked up automatically, and the Themes page paginates at six per screen.

## Credits

- **SilverX** — creator and developer — [github.com/SilverXdamodder](https://github.com/SilverXdamodder)

## License

Licensed under **Creative Commons Attribution-NoDerivatives 4.0 International (CC BY-ND 4.0)**. See [LICENSE](LICENSE). You may share this work with attribution, but you may not distribute modified versions of it.

### Terms of Use

- **All Rights Reserved.** You may use this source only if you credit the original owner. Do not claim it as your own.
- You may use the source if you **make your project open source** and **credit the original owner** — otherwise this menu will be struck down.

---

© SilverX. All rights reserved.
