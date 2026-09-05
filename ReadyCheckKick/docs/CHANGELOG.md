# Changelog

# [Unreleased] 0.6.3

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu registration to the PiCore `AddGenericModConfigMenu` builder API (removed the per-mod registration class); no gameplay change
- Fixed the "kick unready farmers" section title to use its own translation key (it previously showed the "show unready farmers" title)