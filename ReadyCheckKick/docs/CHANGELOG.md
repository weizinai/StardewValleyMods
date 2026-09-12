# Changelog

# [Unreleased]

## Added

- A panel listing the players who are not ready during ready checks (on the host's screen): a portrait and name per row, a "Kick" button on each row, a "Kick
  All" button at the bottom, and mouse-wheel scrolling when the list is longer than the panel. The row button kicks one player per click and the bottom button
  kicks everyone listed; neither asks for confirmation

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal); no gameplay change
- Both unready-player displays moved to PiCore overlay panels (the flat panel look the other mods use): the ready-check list now sits centered above the vanilla
  "Waiting for other players..." dialog instead of as bare red text in the top-left corner, and the overnight-save list keeps its top-right corner but became a
  **host-only** panel — previously every player, guest included, saw who was not ready during the overnight save pass — and it lists in-game display names
  rather than the underlying farmer names and no longer lists the host themselves (vanilla marks the host's own status as `shipment`, not `ready`, on nights
  with anything to ship, so the host used to appear in their own "not ready" list). That overnight panel stays information-only: it has no buttons, so nobody
  can be kicked while the save is in progress
- Removed the mod's only Harmony patch (it drew the overnight-save list) along with the reflection that read vanilla's overnight status table, and with it the
  project's Harmony dependency — the mod no longer loads Harmony at all. The ready-check list still reads vanilla's private ready-state table by reflection,
  because vanilla has no public per-player query for it

## Removed

- The `kup` console command, which kicked every player who was not ready and was previously the only way to kick by hand. Manual kicking now goes through the
  panel buttons, so it requires the "Show Info In Ready Check Dialogue" option to stay on: with that option off the panel and its buttons stay hidden and auto
  kick becomes the only way anyone can be kicked (the option's config-menu tooltip says the same)

## Fixed

- Fixed the "kick unready farmers" section title to use its own translation key (it previously showed the "show unready farmers" title)
- Fixed the "Special Treat For Festival" toggle missing from the GMCM config menu; the option only existed in `config.json` and the translation keys, so it had
  to be enabled by hand-editing the file