# StardewValleyMods

weizinai 的 Stardew Valley（SMAPI）模组合集，面向 SDV 1.6（`net6.0`）；全部代码遵循 MIT 协议（见 [LICENSE](LICENSE)）。发布在
[我的 Nexus 主页](https://next.nexusmods.com/profile/weizinai/mods?gameId=1303)。

## 安装

1. 安装 [SMAPI](https://smapi.io/)。
2. 安装 [PiCore](https://www.nexusmods.com/stardewvalley/mods/33525)：本仓库除 PiCore 外的所有代码模组都以它为运行时依赖，**必须先装**。
3. 从 [Nexus](https://next.nexusmods.com/profile/weizinai/mods?gameId=1303) 下载想用的模组，解压后把模组文件夹整个放进游戏的 `Mods` 目录。

部分模组可选接入 [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)（GMCM）：装上它就能在游戏内调整这些模组的配置，不装也能正常游玩。

## 模组清单

除 PiCore 外，所有代码模组都以 PiCore 为 **运行时依赖**（用户需另行安装）。

| 模组                                              | 说明                            | Nexus                                                       | 文档                                         |
| ------------------------------------------------- | ------------------------------- | ----------------------------------------------------------- | -------------------------------------------- |
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

表中 `—` 表示该模组未在 Nexus 发布，或没有对应的模组文档。

内容包：

- [[CP] More Size Cabin](%5BCP%5D%20More%20Size%20Cabin/) — 更多尺寸的小屋（依赖
  CustomCabinFix），[Nexus 26349](https://www.nexusmods.com/stardewvalley/mods/26349)
- [CP] More Experience（MoreExperience 附带）— MoreExperience 的内容包，依赖 MoreExperience

## 构建与部署

- 前置：.NET SDK（`net6.0`）+ 装有 SMAPI 的 Stardew Valley。
- 按需构建：`dotnet build <模组目录>`（如 `dotnet build AutoBreakGeode`）只构建改动的模组，并把该模组装进本机游戏的 `Mods` 目录，不必每次构建整个解决方案；项目引用的共享库（如
  PiCore）会随之自动构建。发布 zip 输出到仓库根 `.releases/`（隐藏目录）。

## 仓库结构

- 每个模组一个顶层目录；`PiCore/` 是核心库模组（其余模组的运行时依赖），日志与通用代码均由 PiCore 承担。
- `docs/` 是仓库级文档：`docs/vendor/` 存放各参考文档的离线镜像，`docs/agents/` 存放 agent 工作文档（供自动化协作者使用）。
