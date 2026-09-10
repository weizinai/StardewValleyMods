# Changelog

# [Unreleased]

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated multiplayer hint messages (Broadcaster) to PiCore; peers still see them attributed to the sender mod
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu
  carrying the original hard-coded Chinese labels unchanged since this mod has no i18n, hotkey-driven menu opening kept via the module's menu handle); no
  gameplay change

## Removed

- Removed the commented-out spend-limit (花钱限制) dead code across config/menu/patcher/rebuild and dropped the orphaned "Kick Unready Player" option and its
  keybind (no handler exists); the option's stored value resets and its GMCM section is gone