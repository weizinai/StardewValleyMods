# Changelog

# [Unreleased] 2.0.5

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu registration to the PiCore `AddGenericModConfigMenu` builder API (removed the per-mod registration class); no gameplay change