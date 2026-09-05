# Changelog

# [Unreleased] 0.6.1

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated multiplayer hint messages (Broadcaster) to PiCore; peers still see them attributed to the sender mod
- Migrated the config menu registration to the PiCore `AddGenericModConfigMenu` builder API (removed the per-mod registration class); no gameplay change