# Changelog

# [Unreleased]

## Added

- Mod changes are now reported in three types instead of only removals: the hover summary says how many mods were removed, added and updated, and the details
  window lists them under "Removed" / "Added" / "Updated" sections, showing only the sections that actually have entries. Updated mods show their old and new
  version
- Because additions and version changes now count as changes too, a save whose mods were only added or only updated also gets the icon
- The details window now starts with a line giving when the record was written, the game version at that time, and how the mod count changed (then -> now)

## Changed

- Migrated logging to PiCore (removed the Common shared project); console logs still show each mod's own name
- The save list's icons, hover summary and mod-changes window are now drawn by a PiCore UI overlay, and this mod no longer applies any Harmony patches to the
  vanilla save menu—so it no longer conflicts with other mods that change that menu. The icon is still drawn next to the save name in the same spot and with the
  same hitbox, and clicking a save slot still loads that save; the summary is no longer a vanilla tooltip, so it no longer uses (and no longer overwrites) the
  field vanilla reserves for the delete button's hint
- A save slot's icon can now be clicked to open a window with the mod changes; while that window is open no click reaches the save slots behind it, so reading
  the details can never start loading a save
- The record file is now self-describing: it stores the recording time, the game version, and each mod's name and version. Records written by an older version
  are treated as "no mod information recorded" and cannot be compared—load and save that save once to write the new format