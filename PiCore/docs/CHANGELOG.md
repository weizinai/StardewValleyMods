# Changelog

# [Unreleased] 0.5.0

- The config module's member contract is public writable properties only: `ConfigService` reset and `ConfigMember` binding accept only members with a public
  setter (config classes must use public auto-properties and never declare indexers); public fields and indexer members are neither bound nor reset
- Added generic per-mod logging (`Logger<T>` / `Broadcaster<T>`), so SMAPI console logs keep each mod's own source name
- Split HUD messages into their own `HudLogger` / `HudBroadcaster`; uninitialized log calls now fall back to PiCore instead of crashing
- Centralized multiplayer message subscription in PiCore `ModEntry` (`MessageData` is now public)
- Patch failure errors now show the owning mod's name
- Added `Name` and `IsEnabled` to `IPatcher` (with `BasePatcher` defaults); `HarmonyPatcher` skips disabled patchers
- Added single-line `Patch<T>` / `PatchConstructor<T>` binding helpers and a `PatchKind` enum; `GetHarmonyMethod` now fails fast with a clear error when a patch
  method is missing or non-static
- Fixed the `BaseIntegration<TApi>` min-version gate so the API is only fetched after the base version check passes
- Integration warnings now use the consuming mod's own `IMonitor` instead of PiCore's logger
- Aligned the GMCM layer with the official 1.16 API: `IGenericModConfigMenuApi` replaced with the full official interface (adds `AddSubHeader` and
  `OpenModMenuAsChildMenu`; `TryGetCurrentMenu`'s `mod`/`page` are now nullable), GMCM min version raised to 1.16.0, and
  `GenericModConfigMenuIntegration<TConfig>` gained the missing forwarding members (`AddSubHeader`/`AddImage`/`AddKeybind`/`AddComplexOption` etc.)
- Added the config module under `PiCore/Config/` (namespace `weizinai.StardewValleyMod.PiCore.Config`): `ConfigService<TConfig>` owns config read (with
  corrupt-config self-heal reset), GMCM register-on-launch on `GameLaunched`, write-on-save, and fires a single `onConfigChanged` callback after save and reset;
  reset writes `new TConfig()` defaults into the current instance in place, so captured config references (e.g. patchers in AutoBreakGeode /
  FriendshipDecayModify) see the reset without a restart. A companion member-bound declarative descriptor `ConfigMenuDescriptor<TConfig>` (`ConfigMenuSection`
  for nested sub-config sections) binds bool / int / float / text / enum (localized value labels) / keybind-list options, with section titles (a heading bool
  option's label doubles as its section title), pages, page links and paragraphs, and `AddCustomSection` as an escape hatch receiving the raw
  `GenericModConfigMenuIntegration<TConfig>`
- Removed each consuming mod's own GMCM registration class; config menus now register through the config module, leaving exactly one way of writing a config
  menu in the repo
- Fixed `PositionHelper` reading the viewport once at type load; screen↔world conversions now use the live viewport each call, so they no longer drift once the
  viewport scrolls with the player