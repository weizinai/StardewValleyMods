# Changelog

# [Unreleased] 2.0.5

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu with vanilla/RSV pages and the shared quest-config section primitive for the nested quest sub-configs, the `ExcludeNPCList` list-of-string option kept as comma-separated text via the escape hatch); no gameplay change