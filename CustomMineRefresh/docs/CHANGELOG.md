# Changelog

# [Unreleased]

## Added

- New: volcano dungeon floors 1–9 with no players in them are refreshed on their own cadence (default: every 60 game minutes); the entrance level is never
  refreshed

## Changed

- Breaking: the mod is renamed to `Custom Mine Refresh` and its `UniqueID` is now `weizinai.CustomMineRefresh`; delete the old `AutoRefreshMineshaft` folder before installing
  this version. The new name says what the mod is: the refresh strategy you configure (`Custom`), covering both the mine and the volcano dungeon (`Mine`)
- Breaking: only the host needs the mod now — clients need nothing at all, not even SMAPI. It works in single-player too
- Mine floors with no players in them are refreshed as soon as they are unoccupied, so going down a floor and back up hands you a fresh one. Much stronger than
  vanilla: in multiplayer vanilla never cleans up when you leave the mine (and its 10-minute check keeps every floor down to the deepest one in use), and in
  single-player you still have to leave the whole mine first
- GMCM now shows two independent groups, one per dungeon, each with its own switch and cadence: turning one off leaves the other alone. The cadence is in game
  minutes (10 game minutes ≈ 7 seconds), `0` means immediately (checked every second), and the maximum is 10 game hours
- GMCM 1.16.0 or later is now required (the menu runs on the PiCore `ConfigService`/descriptor module); warnings show the mod's own name