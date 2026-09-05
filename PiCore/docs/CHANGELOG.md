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