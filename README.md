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

Settings changed inside the menu (theme, scale, opacity, binds) are persisted separately via `PlayerPrefs`.
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
