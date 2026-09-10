# Changelog

# [Unreleased]

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu with
  the eight feature pages and page links, the shared nested-object section primitive rendering the three-level automation sub-configs, the buff-type options
  kept with localized labels and string↔enum mapping, and the tree growth-stage dictionary options kept via the escape hatch); no gameplay change