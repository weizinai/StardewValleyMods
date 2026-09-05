# CHANGELOG

# [待定] 0.5.0

- Added generic per-mod logging (`Logger<T>` / `Broadcaster<T>`) with a monitor registry, so SMAPI console logs keep each mod's own source name
- Split HUD messages into the non-generic `HudLogger` / `HudBroadcaster`; uninitialized log calls now fall back to PiCore instead of crashing
- Made `MessageData` public and centralized multiplayer message subscription in PiCore `ModEntry`
- Patch failure errors now show the owning mod's name

## [0.4.0] 2025-05-09

### Added

- Add property accessor methods
- Introduced string constant classes to accurately represent in-game items, monsters and NPCs
