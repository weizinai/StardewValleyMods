# Changelog

# [Unreleased] 1.7.2

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, enum menu-tab option
  now via the module's localized enum option); no gameplay change
- Rebuilt the in-game menu on the PiCore UI framework: the menu now supports controller (gamepad) navigation, tabs are laid out across the top, and
  paging uses a button strip at the bottom; art tiles are slightly smaller to fit a 720p viewport. The tabs, per-page 3×3 grid content and favorites
  behavior are unchanged.
- Restored the Community Center option on the Town tab and reworked it: clicking it now opens the vanilla bundle
  menu from any location — the same entry as the inventory-page Community Center icon, auto-selecting the first room with bundles still to complete
  and including the room-switching arrows. The option still stays hidden on the Joja route and until the forest-vision event has been seen.
- Breaking: the `HarveyOption` and `ForgeOption` catalog ids were renamed to `Harvey`/`Forge` to match the other option ids
  (they are persisted in `config.json`); any existing favorite or default-tab entry for these two must be re-added.