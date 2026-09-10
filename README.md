# StardewValleyMods

weizinai 的 Stardew Valley（SMAPI）模组合集，面向 SDV 1.6（`net6.0`），全部代码遵循 MIT 协议（见 [LICENSE](LICENSE)）。

## 模组列表

除 PiCore 外，所有代码模组都以 PiCore 为 **运行时依赖**（用户需另行安装）。

| 模组                                              | 说明                            | Nexus                                                       | 文档                                         |
|---------------------------------------------------|---------------------------------|-------------------------------------------------------------|----------------------------------------------|
| [ActiveMenuAnywhere](ActiveMenuAnywhere/)         | 任意地点激活各类菜单            | [21093](https://www.nexusmods.com/stardewvalley/mods/21093) | [docs](ActiveMenuAnywhere/docs/README.md)    |
| [AutoBreakGeode](AutoBreakGeode/)                 | 自动砸晶球                      | [20685](https://www.nexusmods.com/stardewvalley/mods/20685) | [docs](AutoBreakGeode/docs/README.md)        |
| [BetterCabin](BetterCabin/)                       | 让小屋拥有更多功能              | [25368](https://www.nexusmods.com/stardewvalley/mods/25368) | [docs](BetterCabin/docs/README.md)           |
| [CustomCabinFix](CustomCabinFix/)                 | 修复添加自定义小屋时出现的问题  | [26979](https://www.nexusmods.com/stardewvalley/mods/26979) | [docs](CustomCabinFix/docs/README.md)        |
| [CustomMineRefresh](CustomMineRefresh/)           | 按配置刷新无人的矿井与火山楼层  | [33451](https://www.nexusmods.com/stardewvalley/mods/33451) | [docs](CustomMineRefresh/docs/README.md)     |
| [FastControlInput](FastControlInput/)             | 加快游戏对玩家输入的反应        | [24324](https://www.nexusmods.com/stardewvalley/mods/24324) | [docs](FastControlInput/docs/README.md)      |
| [FreeLock](FreeLock/)                             | 自由移动视角                    | [25329](https://www.nexusmods.com/stardewvalley/mods/25329) | [docs](FreeLock/docs/README.md)              |
| [FriendshipDecayModify](FriendshipDecayModify/)   | 调整好感度衰减值                | [23862](https://www.nexusmods.com/stardewvalley/mods/23862) | [docs](FriendshipDecayModify/docs/README.md) |
| [HelpWanted](HelpWanted/)                         | Help Wanted 悬赏系统的 1.6 版本 | [21766](https://www.nexusmods.com/stardewvalley/mods/21766) | —                                            |
| [LazyMod](LazyMod/)                               | 挂机助手，自动完成许多日常操作  | [22826](https://www.nexusmods.com/stardewvalley/mods/22826) | [docs](LazyMod/docs/README.md)               |
| [MoreExperience](MoreExperience/)                 | 增加更多获得经验的途径          | —                                                           | [docs](MoreExperience/docs/README.md)        |
| [MultiHost](MultiHost/)                           | 同时托管多个农场                | —                                                           | —                                            |
| [MultiplayerModLimit](MultiplayerModLimit/)       | 限制房客可使用的模组            | [25446](https://www.nexusmods.com/stardewvalley/mods/25446) | [docs](MultiplayerModLimit/docs/README.md)   |
| [PiCore](PiCore/)                                 | 核心库，供作者其余模组复用      | [33525](https://www.nexusmods.com/stardewvalley/mods/33525) | [docs](PiCore/docs/README.md)                |
| [ReadyCheckKick](ReadyCheckKick/)                 | 过夜时在右上角显示未准备的玩家  | [33527](https://www.nexusmods.com/stardewvalley/mods/33527) | [docs](ReadyCheckKick/docs/README.md)        |
| [SaveModInfo](SaveModInfo/)                       | 显示存档的模组变动信息          | [25903](https://www.nexusmods.com/stardewvalley/mods/25903) | [docs](SaveModInfo/docs/README.md)           |
| [SomeMultiplayerFeature](SomeMultiplayerFeature/) | 一些联机功能                    | —                                                           | —                                            |
| [SpectatorMode](SpectatorMode/)                   | 旁观者模式                      | [25339](https://www.nexusmods.com/stardewvalley/mods/25339) | [docs](SpectatorMode/docs/README.md)         |
| [TestMod](TestMod/)                               | 测试用模组                      | —                                                           | —                                            |

内容包：

- [[CP] More Size Cabin](%5BCP%5D%20More%20Size%20Cabin/) — 更多尺寸的小屋（依赖
  CustomCabinFix），[Nexus 26349](https://www.nexusmods.com/stardewvalley/mods/26349)
- [CP] More Experience（MoreExperience 附带）— MoreExperience 的内容包，依赖 MoreExperience

## 仓库结构

- 每个模组一个顶层目录，且遵循统一的[开发约定](#模组开发约定)；`PiCore/` 是核心库模组（其余模组的运行时依赖），日志与通用代码均由 PiCore 承担。
- `docs/` 是仓库级文档：`docs/vendor/` 存放各参考文档的离线镜像，`docs/agents/` 存放 agent 工作文档（供自动化协作者使用）。

## 构建与部署

- 前置：.NET SDK（`net6.0`）+ 装有 SMAPI 的 Stardew Valley；ModBuildConfig 会自动探测游戏路径。
- 按需构建：`dotnet build <模组目录>`（如 `dotnet build AutoBreakGeode`）只构建改动的模组并自动部署进游戏 `Mods` 目录，不必每次构建整个解决方案；项目引用的共享库（如
  PiCore）会随之自动构建。发布 zip 输出到仓库根 `.releases/`（隐藏目录）。
- 版本号唯一来源是各 csproj 的 `<Version>`（manifest 用 `%ProjectVersion%` 占位）；改版本只改 csproj。
- 每个模组的详细介绍、更新日志（英文 `CHANGELOG.md` 与中文 `CHANGELOG.zh.md`）、截图放在各自 `docs/` 目录。

## 模组开发约定

新建或修改模组时遵循：

- **统一骨架**：`ModEntry.cs`（`Entry` 里 `I18n.Init`、注册事件、调用 `HarmonyPatcher.Apply`）+ `Framework/`（ModConfig 与 GMCM 集成类）+ `Patcher/`（Harmony patch）+
  `Handler/`（功能实现）+ `i18n/` + `docs/`（README.md、CHANGELOG.md、CHANGELOG.zh.md、screenshots/）+ `manifest.json`。任意现有模组目录都是一份范例。
- **复用基建**：PiCore 提供 `Patcher`（含自带异常隔离的 `HarmonyPatcher`）、`Handler`、`Integration`（含 Generic Mod Config Menu 集成）、`Extension`、`Constant`
  ；优先复用，再自行实现。
- **PiCore 依赖**：`ProjectReference` 一律 `Private="false"`（不把 PiCore.dll 打进发布包），并在 manifest `Dependencies` 中声明 `weizinai.PiCore` 必装。
- **翻译**：`i18n/default.json` 是英文基文案、`zh.json` 是中文译文；键名按 `Config.*` / `UI.*` 分段；ModTranslationClassBuilder 会把键生成成强类型 `I18n`
  类，键写错会在编译期报错。