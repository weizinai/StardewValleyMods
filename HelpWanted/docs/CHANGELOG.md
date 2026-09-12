# Changelog

# [Unreleased]

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu with
  vanilla/RSV pages and the shared quest-config section primitive for the nested quest sub-configs, the `ExcludeNPCList` list-of-string option kept as
  comma-separated text via the escape hatch); no gameplay change
- Rebuilt the vanilla billboard and RSV quest board menus on the PiCore.UI framework; the billboard, the notes and the decorations look the same as before, and
  quest notes no longer go missing when the board gets crowded: every pending quest is posted, overlapping each other when they cannot all fit (previously the
  notes that did not fit were silently dropped). Differences a player can notice: the keyboard arrow keys no longer move the focus between notes (Esc still steps
  back from a quest and closes the board), the Accept button's hover look becomes the framework's pale-yellow fill with a gold ring (it used to grow and turn
  pink), hovering a note now draws a gold highlight and plays the hover sound, and clicking a note now plays a confirm sound; note hover and note clicks were
  both silent before. No settings need to be redone.