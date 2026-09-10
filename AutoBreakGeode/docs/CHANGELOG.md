# Changelog

# [Unreleased]

## Changed

- Migrated the config menu to the PiCore `ConfigService`/descriptor config module (static `ModConfig.Instance`); "Reset to defaults" now takes effect
  immediately instead of after a restart
- The GeodeMenu begin button now uses the shared flat UI style — bigger, with a hover highlight and hover/click sounds — and shows the current state ("Begin" /
  "Stop"), so you can tell at a glance whether auto-breaking is on; it also flips back to "Begin" when auto-breaking stops on its own (out of gold, with a full
  inventory, or out of geodes)
- The keybind only applies inside the GeodeMenu, and does nothing while you are not holding a geode (instead of appearing to do nothing)
- The "Geode Animation Speed" option is now a 1–20 slider (it used to be an unbounded text box that accepted 0, negative or huge values straight into
  `config.json`); it only applies when Fast Animations is not installed — with that mod the animation speed is entirely up to it, so this option is no longer
  shown; a value of 1 keeps the vanilla speed
- The button sits just outside the left edge of the geode panel, level with the geode-spot top, and stays there when the window is resized; the held-item icon
  draws on top of the button
- Renaming a config option resets the "Toggle Auto Break Keybind" setting to its default (F) when you upgrade; remap it once after updating if you had changed
  it

## Removed

- The button is now always drawn in the GeodeMenu: the "Draw Begin Button" option was removed, so it no longer has to be enabled first

## Fixed

- Running out of gold to pay the 25g per geode now shakes the money box and shows the "not enough gold" cue like the vanilla game does, then stops
  auto-breaking; previously it got stuck showing "Stop" while unable to crack anything, with the money box shaking forever
- A full inventory now shows the vanilla "inventory full" cue and stops auto-breaking; previously auto-breaking gave up as soon as only one inventory slot was
  free — which silently cancelled even the case the vanilla game allows (holding the very last geode), leaving you with no explanation
- Auto-breaking now pauses while the game is paused or the window loses focus (matching the game's own "pause when out of focus"), and resumes where it left off
  when you come back, keeping the toggle state; previously it kept cracking while unfocused, where key presses no longer reach the game, so you could not stop
  it
- One click on the button now toggles auto-breaking exactly once (holding it down no longer flips it every frame), and that click belongs to the button alone:
  it no longer cracks a geode as a side effect