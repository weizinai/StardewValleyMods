# Changelog

# [Unreleased] 0.5.0

- Added generic per-mod logging (`Logger<T>` / `Broadcaster<T>`) with a monitor registry, so SMAPI console logs keep each mod's own source name
- Split HUD messages into the non-generic `HudLogger` / `HudBroadcaster`; uninitialized log calls now fall back to PiCore instead of crashing
- Made `MessageData` public and centralized multiplayer message subscription in PiCore `ModEntry`
- Patch failure errors now show the owning mod's name
- Added `Name` and `IsEnabled` to `IPatcher` (with `BasePatcher` defaults); `HarmonyPatcher` skips disabled patchers and logs per-patch success plus an apply summary
- Replaced `HarmonyPatcher.Apply(string, ...)` with a single `Apply(Mod, ...)` entry point
- Added single-line `Patch<T>` / `PatchConstructor<T>` binding helpers and a `PatchKind` enum; `GetHarmonyMethod` now fails fast with a clear error when a patch method is missing or non-static
- Patcher dependencies are now held by a static singleton instance instead of copied into static fields
- Fixed the `BaseIntegration<TApi>` min-version gate so the API is only fetched after the base version check passes
- Integration warnings now use the consuming mod's own `IMonitor` instead of PiCore's logger
- Replaced `IGenericModConfigMenuApi` with the official GMCM 1.16 interface (adds `AddSubHeader`/`AddKeybind`/`OpenModMenuAsChildMenu`, nullable `TryGetCurrentMenu`) and raised the GMCM min version to 1.16.0
- Fixed the GMCM integration interface: `AddComplexOptionWithGamepadSupport` doesn't exist in GMCM 1.16.0 (the gamepad variant was only added in 1.17), which broke Pintail's API mapping at startup; the interface now declares GMCM 1.16.0's 12-parameter `AddComplexOption`
- Completed the `GenericModConfigMenuIntegration<TConfig>` forwarding members; `AddGenericModConfigMenu` now takes a builder delegate instead of a registration class