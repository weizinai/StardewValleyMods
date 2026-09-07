# Changelog

# [Unreleased] 1.4.2

- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`); "Reset to defaults" now takes effect
  immediately instead of after a restart