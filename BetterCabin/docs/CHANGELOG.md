# Changelog

# [Unreleased] 0.11.1

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu with nested offset sub-config sections via `AddSection`); "Reset to defaults" now takes effect immediately in-game instead of after a restart