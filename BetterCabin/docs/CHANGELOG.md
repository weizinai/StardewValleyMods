# Changelog

# [Unreleased]

## Added

- The client cabin appearance panel now supports focus-ring navigation for controllers: the stick or d-pad moves the focus, A activates the focused button and
  B closes the panel

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- Migrated Harmony patches to the PiCore `Patcher` API (single-line `Patch<T>` bindings); no gameplay change
- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`, corrupt-config self-heal, declarative menu with
  nested offset sub-config sections via `AddSection`); "Reset to defaults" now takes effect immediately in-game instead of after a restart
- Reworked the three cabin labels (owner name / total online time / last online time) to draw through PiCore's world-anchoring path: they are now flat (no
  panel drop shadow, no text shadow), draw on top of the world content instead of being covered by buildings drawn after them, and are clipped away once their
  cabin scrolls out of view. Each label keeps its own toggle and offset settings, every config key and offset unit is unchanged, and the labels no longer
  allocate an object per cabin per frame
- Changed the default offline colour for a cabin owner's name from white to grey, so it reads clearly on the label's beige panel while staying distinct from
  the black used for online players. The new default only applies where no colour was saved: if your `config.json` already stores an `OfflineFarmerColor`,
  delete that entry to pick up the grey
- The config menu no longer repeats a section title on its switch: each section keeps its descriptive title, and the switch that turns the feature on is now
  labelled "Enable", so the same sentence is no longer shown twice; hover tooltips keep the original wording
- Reworked the client cabin appearance panel into a PiCore.UI menu: it is now flat (no panel or text drop shadow) and shows the cabin in a large 4× preview
  flanked by the vanilla left/right arrows (each with a hover tooltip the old panel did not have) with an appearance counter like "3 / 7" below; the old move
  and close icon buttons are now the "Move Cabin" (with the game's own "Move Buildings" tooltip) and "Close" text buttons. The panel is 760px wide and its
  height follows the preview, capped to your screen height and centred with the overflow cropped if it still does not fit. Switching appearance still applies
  immediately and syncs to the host, and moving the cabin is unchanged. The panel's sounds now come from PiCore's theme: hovering and clicking a button plays
  the shared hover/confirm cue instead of the old panel's two one-off effects (one when switching appearance, one when entering the move mode), and Close plays
  that one cue

## Fixed

- The prompt shown when resetting a cabin now asks which players to reset, matching the list of players it opens (it previously asked for a cabin)
- English wording is cleaned up in two places: resetting a cabin no longer puts a stray "The" before the player's name, and the message for adding a player to
  a cabin's whitelist no longer contains a full-width Chinese comma (its "you cabin" typo is corrected to "your cabin")