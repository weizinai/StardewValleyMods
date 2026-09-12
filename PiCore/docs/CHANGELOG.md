# Changelog

# [Unreleased]

## Added

- Added generic per-mod logging (`Logger<T>` / `Broadcaster<T>`), so SMAPI console logs keep each mod's own source name
- Added `Name` and `IsEnabled` to `IPatcher` (with `BasePatcher` defaults); `HarmonyPatcher` skips disabled patchers
- Added single-line `Patch<T>` / `PatchConstructor<T>` binding helpers and a `PatchKind` enum; `GetHarmonyMethod` now fails fast with a clear error when a patch
  method is missing or non-static
- Added the config module under `PiCore/Config/` (namespace `weizinai.StardewValleyMod.PiCore.Config`): `ConfigService<TConfig>` owns config read (with
  corrupt-config self-heal reset), GMCM register-on-launch on `GameLaunched`, write-on-save, and fires a single `onConfigChanged` callback after save and reset;
  reset writes `new TConfig()` defaults into the current instance in place, so captured config references (e.g. patchers in AutoBreakGeode /
  FriendshipDecayModify) see the reset without a restart. A companion member-bound declarative descriptor `ConfigMenuDescriptor<TConfig>` (`ConfigMenuSection`
  for nested sub-config sections) binds bool / int / float / text / enum (localized value labels) / keybind-list options, with section titles (a heading bool
  option's label doubles as its section title), pages, page links and paragraphs, and `AddCustomSection` as an escape hatch receiving the raw
  `GenericModConfigMenuIntegration<TConfig>`
- **UI framework — foundation & menu host:** retained layout kernel under `PiCore/UI` (namespaces `weizinai.StardewValleyMod.PiCore.UI*`): WPF-style two-pass
  measure/arrange `Element` with dirty invalidation, and `LayoutRunner` flushes pending layout before any bounds are read for screen positioning; a thin flat
  `Theme` (9-slice menu box, close button, fonts, palette, sound names, flat panel / flat text / mouse-cursor draw helpers); and `MenuHost.OpenMenu(root)` runs
  the vanilla menu pipeline — dimmed backdrop, flat chrome, root view, upper-right X close, mouse drawn last. Keyboard contract: Esc closes
  (`readyToClose()` → `exitThisMenu()`), right-click does nothing, and vanilla direction-key snap movement stays off
- **Layout containers & core widgets:** `Stack` (vertical/horizontal, content-adaptive), `Grid` (fixed cells, whole block centered) and `Canvas` (free absolute
  placement) under `PiCore/UI/Layout`; a single-line `Label` (measured and drawn with the game-language SpriteFont, so measured width equals drawn width) with
  optional `MaxWidth` text wrapping via the shared `TextWrap` rules — CJK per character, Latin per word, trailing punctuation glued to its line, `\n` as a hard
  break, multi-line height pushing following elements down; a `Button` (flat 9-slice + text, hover visual and sound, `OnClick`); and a `PanelFrame` 9-slice
  chrome that insets its single content. `MenuHost` routes the mouse to the topmost visible button: hover sets/restores its state and plays the hover sound,
  left click fires `OnClick` exactly once (after the X close check)
- **Focus graph & gamepad navigation:** `FocusManager` / `FocusDirection` build a focus graph over focusable widgets and move focus by the validated geometry
  rules (center-point axial half-plane, same-row priority for Left/Right, smallest cross-axis offset when equidistant), rebuilding on structure change while
  keeping the current focus. `MenuHost` polls the gamepad directly: stick/DPad moves focus (instant press + held repeat), A activates the focused item exactly
  once per press, B closes, the game cursor snaps onto the focused item (mouse motion hands control back), and pad input suppresses mouse hover.
  `areGamePadControlsImplemented()` returns true so SDV adds no extra click per A; `Element` exposes `Focusable` / `ActivateAction` / `IsFocused`, and `Button`
  is focusable with a gold focus ring
- **`Scrollable` list panel:** a fixed-size scissor-clipped viewport that stacks its children vertically, folding the scroll offset into child bounds at arrange.
  Content is inset from the 9-slice border and both drawing and hit-testing clip to the inner viewport — list items never sit on the border, and scrolled-out
  items are neither drawn nor clickable (so a long list never blocks the elements above it). `MenuHost` scrolls it from the mouse wheel over the list or the pad
  right stick; the focus graph auto-scrolls a focused item into view (Up/Down walk the list's own items first, leaving only at its first/last item)
- **`Tooltip`:** an element's `TooltipText` draws a flat tooltip above the content. Mouse-driven tooltips follow the cursor and disappear as soon as the mouse
  leaves; pad-driven tooltips anchor beside the focused item and follow focus. Placement avoids the item and its gold focus ring (cursor-side placement flips
  to the other side on screen edges), and the body wraps at a max inner width (the same `TextWrap` contract as `Label`)
- **`TabControl` & `Pager`:** `TabControl` owns a horizontal rail of selectable chips plus a content area — clicking a chip or pressing A swaps the mounted
  content cleanly (the focus graph rebuilds and keeps the current chip), content implementing `IResettable` (e.g. a `Pager`) resets on switch, and the selected
  chip reads as a translucent box. `Pager` flips between page content elements with Previous/Next buttons and an auto-updating page label (hidden at the
  first/last page). `MenuHost` flushes pending layout at the start of draw, so content swapped inside a handler lands cleanly the same frame
- **Overlay & world-anchored hosts:** `DrawableHost` draws the same retained root on RenderedHud, RenderedActiveMenu or a single render step
  (`CreateDrawable(root, display, RenderSlot, position)` / `CreateDrawableOnStep`), read-only by default; hover, left-click and wheel routing are opt-in — the
  consumer calls `PerformHoverAction(x, y)` (puts the topmost visible `Button` in hover state and plays the hover sound), `HandleLeftClick(x, y)` (fires its
  `OnClick` with the accept sound and returns `true` on a hit, so the consumer can swallow that click before the vanilla menu handles it — an overlay button and
  the clickable area underneath it no longer both fire) and `PerformScrollAction(direction)` (scrolls the topmost visible `Scrollable` under the cursor by whole
  notches of 48px with the vanilla sign convention; the host reads the cursor position itself, so a consumer cannot pass screen pixels off as UI coordinates);
  `WorldAnchorHost.Create(display, worldAnchor, content, placement, offset)` anchors content to an absolute world position (e.g. the player) on RenderedWorld,
  tracking the live viewport and culling fully off-screen boxes; `TilePanel` is the anchored flat title/body panel (DialogueFont title, SmallFont body lines).
  Both hosts subscribe on creation, lay the root out by its content size, and `Disable()`/`Enable()` cleanly (no ghost draw); overlays on an active menu redraw
  the mouse cursor on top

## Changed

- The config module's member contract is public writable properties only: `ConfigService` reset and `ConfigMember` binding accept only members with a public
  setter (config classes must use public auto-properties and never declare indexers); public fields and indexer members are neither bound nor reset
- Split HUD messages into their own `HudLogger` / `HudBroadcaster`; uninitialized log calls now fall back to PiCore instead of crashing
- Centralized multiplayer message subscription in PiCore `ModEntry` (`MessageData` is now public)
- Patch failure errors now show the owning mod's name
- Integration warnings now use the consuming mod's own `IMonitor` instead of PiCore's logger
- Aligned the GMCM layer with the official 1.16 API: `IGenericModConfigMenuApi` replaced with the full official interface (adds `AddSubHeader` and
  `OpenModMenuAsChildMenu`; `TryGetCurrentMenu`'s `mod`/`page` are now nullable), GMCM min version raised to 1.16.0, and
  `GenericModConfigMenuIntegration<TConfig>` gained the missing forwarding members (`AddSubHeader`/`AddImage`/`AddKeybind`/`AddComplexOption` etc.)

## Removed

- Removed each consuming mod's own GMCM registration class; config menus now register through the config module, leaving exactly one way of writing a config
  menu in the repo

## Fixed

- Fixed the `BaseIntegration<TApi>` min-version gate so the API is only fetched after the base version check passes
- Fixed `PositionHelper` reading the viewport once at type load; screen↔world conversions now use the live viewport each call, so they no longer drift once the
  viewport scrolls with the player