# Changelog

# [Unreleased]

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal); no gameplay change

## Fixed

- Fixed the "kick unready farmers" section title to use its own translation key (it previously showed the "show unready farmers" title)