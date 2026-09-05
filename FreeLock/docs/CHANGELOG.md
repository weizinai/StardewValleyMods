# Changelog

# [Unreleased] 0.2.2

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated the config menu registration to the PiCore `AddGenericModConfigMenu` builder API (removed the per-mod registration class); no gameplay change