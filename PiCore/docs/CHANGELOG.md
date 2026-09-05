# Changelog

# [Unreleased] 0.5.0

- Added generic per-mod logging (`Logger<T>` / `Broadcaster<T>`) with a monitor registry, so SMAPI console logs keep each mod's own source name
- Split HUD messages into the non-generic `HudLogger` / `HudBroadcaster`; uninitialized log calls now fall back to PiCore instead of crashing
- Made `MessageData` public and centralized multiplayer message subscription in PiCore `ModEntry`
- Patch failure errors now show the owning mod's name