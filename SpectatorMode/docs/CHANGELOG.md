# Changelog

# [Unreleased] 0.10.3

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (corrupt-config self-heal, declarative menu binding the public auto-property config members via the descriptor); no gameplay change