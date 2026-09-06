# Modding tools

Choose tools for the task and the game's **Windows x64 Mono** runtime. The handbook's local build target is stable **1.0.53a**, Unity **2020.3.49f1**. Recheck runtime and dependency versions when targeting **1.0.57** or another build.

| Task | Tool | Setup |
| --- | --- | --- |
| Build C# mods | .NET SDK **8.0.424**, any suitable C# editor | [Build environment](../tutorials/Build%20Environment.md); the mod target is `net48` |
| Load maintained examples | [Unity Mod Manager](https://github.com/newman55/unity-mod-manager) | Use the installed UMM/Harmony pair; examples compile with UMM 0.33/Harmony 2.3.6 |
| Inspect managed code | [dnSpyEx](https://github.com/dnSpyEx/dnSpy/releases) or [ILSpy](https://github.com/icsharpcode/ILSpy) | Open the game's own `Managed` assemblies read-only; use IL when decompiled C# is ambiguous |
| Inspect live objects/UI | [UnityExplorer](https://github.com/yukieiji/UnityExplorer) or [RuntimeUnityEditor](https://github.com/ManlyMarco/RuntimeUnityEditor) | Match the package to your loader and Mono; see [debugging](debugging.md) |
| Build AssetBundles | [Unity 2020.3.49f1](https://unity.com/releases/editor/whats-new/2020.3.49) | The guide's fixed authoring profile; [bundle workflow](assets.md) |
| Inspect/export assets | [AssetRipper](https://github.com/AssetRipper/AssetRipper) | Start with a small bundle; exported projects may need repair before rebuilding |
| Prepare portrait video | [FFmpeg](https://ffmpeg.org/download.html) | `libvpx`/VP8 and explicit alpha checks; [portraits](../tutorials/Councillor%20Portraits.md) |
| Edit images | [GIMP](https://www.gimp.org/downloads/) | Retain layered sources and export RGBA PNG |
| Author audio | [FMOD Studio](https://www.fmod.com/download#fmodstudio) | Start with the game's supplied project; [audio version and bank guidance](../tutorials/Audio%20Modding%20Guide.md) |
| Author map outlines | [Inkscape](https://inkscape.org/release/) | Inspect saved dimensions, layers and paths; [map constraints](../tutorials/MapCreation.md) |

The original [AssetStudio](https://github.com/Perfare/AssetStudio) and [sinai-dev/UnityExplorer](https://github.com/sinai-dev/UnityExplorer) repositories are archived. Prefer the active tools above when choosing a new setup. A tool's own .NET requirement is independent of the target framework of your mod.

## Choose the loader before the package

UMM mods, BepInEx plugins and MonoMod patches are different package types. A BepInEx 5 plugin is not automatically compatible with BepInEx 6, and an IL2CPP package is wrong for this Mono player. Follow the chosen release's instructions rather than copying multiple startup proxies over one another.

For an existing BepInEx/MonoMod mod, use the [alternate-loader guide](../tutorials/MonoMod%20Guide.md), which covers deployment, patch conventions and the PVC/UMM coexistence setup. For new handbook examples, use [UMM/Harmony](code-modding.md).

## Working references

- [TIShipModdingFramework](https://github.com/UNNRazorback/TIShipModdingFramework): ship asset/code integration; check [prefab and FX requirements](assets.md#inspect-assets-and-ship-examples).
- [Sarah's tech tree](https://sarahwatt.ca/terra-invicta/techtree/?lang=en): browse relationships, checking the dataset's version against your installed templates.
- [TerraInvictaMCP](https://github.com/MeatBunny/TerraInvictaMCP): the original project's setup and documentation.

Use a syntax-aware editor for JSON. Spreadsheet/CSV conversions can flatten arrays or change types and identifiers; validate the reconstructed JSON before deploying it. The repository checks run with `python scripts/validate.py`.

For the spreadsheet workflow, the original guides link [JSON to CSV](https://www.convertcsv.com/json-to-csv.htm) and [CSV to JSON](https://www.convertcsv.com/csv-to-json.htm). Compare the round-trip result with the original nested records before using it. [GIMP's older-release archive](https://download.gimp.org/pub/gimp/) is useful when reproducing an existing image-authoring setup.
