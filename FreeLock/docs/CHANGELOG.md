# Changelog

# [Unreleased]

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`); no gameplay change