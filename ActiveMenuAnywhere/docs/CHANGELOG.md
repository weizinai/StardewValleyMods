# Changelog

# [Unreleased] 1.7.2

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, enum menu-tab option
  now via the module's localized enum option); no gameplay change
- Rebuilt the in-game menu on the PiCore UI framework: the menu now supports controller (gamepad) navigation, tabs are laid out across the top, and
  paging uses a button strip at the bottom; art tiles are slightly smaller to fit a 720p viewport. The tabs, per-page 3×3 grid content and favorites
  behavior are unchanged.