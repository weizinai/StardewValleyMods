# 模组开发参考资料

编写、审查本仓库（Stardew Valley 模组）代码时的一线查证地图。每个关键事实都要能追溯到下面的某个来源；**基于证据动笔，不凭记忆或猜测**。

## 铁律：先查证，后决定

- 任何拿不准的事实（类名、方法签名、行为、版本号）先查到证据再写。写进文档/注释时注明来源（本地路径或 URL），方便复核。
- 查证顺序（本地优先）：本地源码 → 本地镜像（`docs/vendor/`）→ 在线规范文档 → 官方仓库。
- 文档与源码冲突时**以源码为准**（wiki 偶有过期内容），并把冲突回报出来。
- 本机环境事实：`web_fetch` 工具无外网（DNS 被沙箱拦截，报 "resolves to a non-public IP address"）；`pwsh` 的 `Invoke-WebRequest` 有真实出口（已实测 HTTP 200）。在线抓取用 pwsh，离线用 `docs/vendor/` 镜像。
- 镜像快照的溯源信息在 `docs/vendor/README.md`，本文件不重复。

## 引用源地图

### 1. 游戏本体反编译源码 — `D:\Projects\C#\StardewValley\StardewValley`

解剖游戏内部行为的最终依据：字段、方法、事件、菜单、NPC、地点、Netcode 等都在这里。

- 锚点：`StardewValley/Game1.cs`（主类）；功能按命名空间分布：`.Menus`、`.Objects`、`.Locations`、`.Characters`、`.Events`、`.Netcode`、`.Buffs`、`.Enchantments` 等。
- **版本号没有源码常量**：`Game1.version` 等字段在 `Game1` 构造函数（约 2302–2331 行）由程序集 `AssemblyInformationalVersionAttribute` 运行时填充，反编译源码里读不到字面值。需要具体版本事实时查 wiki 迁移指南表或运行时值。

### 2. SMAPI 源码与文档 — `D:\Projects\C#\SMAPI`

SMAPI 规则、接口、事件的权威来源，兼官方总结文档。

- 锚点：`src/SMAPI/IModHelper.cs`（`IModHelper` 接口）；`src/SMAPI/Translation.cs`（**类名是 `Translation`，不是 `ModTranslation`**——`ModTranslation` 只是 NuGet 包名）；`src/SMAPI/Framework/ModHelpers/`（各 helper 实现）。
- 文档：`docs/README.md`（总索引，含 "For modders" 一节）、`docs/mod-build-config.md`、`docs/technical/mod-package.md`（构建配置）。
- `src/` 共 14 个项目：`SMAPI`（核心）、`SMAPI.ModBuildConfig`/`Analyzer`（构建配置与静态分析）、`SMAPI.Tests`、`SMAPI.Web` 等。

### 3. 官方 wiki 模组索引 — https://stardewvalleywiki.com/Modding:Index

- 结构：Using mods / Creating mods（**C# 模组**与**内容包**两条路线）/ Specific topics（游戏机制、NPC、物品数据、地点与地图、迁移指南…）/ See also（兼容性列表、log parser、JSON 校验等工具）。
- 用它的时机：定位"某主题有没有官方文档、常见做法是什么"；具体细节进各专题页（在线）。
- 离线镜像：`docs/vendor/stardew-wiki/`（仅索引页快照，修订版 2025-11-15；专题页在线）。

### 4. Harmony 文档 — https://harmony.pardeike.net/（Harmony 2）

写/审 Harmony patch（Prefix/Postfix/Transpiler/Finalizer）时查这里。

- 关键限制（intro 原文）：只能 patch 有 IL 方法体的方法（含构造器与属性访问器）；过小的方法会被**内联**导致 patch 不生效；不能给类加字段、不能扩展枚举；泛型方法/泛型类内的 patch 可能不按预期工作。
- 两种打法：**注解式**（`[HarmonyPatch]` + 静态 `Prefix`/`Postfix` 方法）或**手动反射式**（`harmony.Patch(...)` + `AccessTools.Method`/`SymbolExtensions.GetMethodInfo`）。
- API 结构：Basics / Targeting / Attributes / Transpiling / Helper classes 分组；`HarmonyLib` 命名空间完整类表（`AccessTools`、`CodeInstruction`、`CodeMatcher`、`Traverse`、`SymbolExtensions`…）。
- 离线镜像：`docs/vendor/harmony-docs/`（intro、API 索引、HarmonyLib 类表三页；各类详细页在线）。

### 5. ModTranslationClassBuilder（翻译键强类型类生成器）

把运行时才校验的翻译查找变成编译期检查的强类型类。

- 作用：由 `i18n/default.json` 自动生成静态类，`helper.Translation.Get("range-value", new { min = 1, max = 5 })` 换成 `I18n.RangeValue(min: 1, max: 5)`——键名或变量名写错当场编译报错。
- 用法要点：NuGet 包 `Pathoschild.Stardew.ModTranslationClassBuilder`（本仓库 14 个模组项目已引用，本地 NuGet 缓存 v2.2.0）；`Entry` 里调 `I18n.Init(helper.Translation)`。
- 生成规则：键 → CamelCase、`.` → `_`（`generic.ready-now` → `I18n.Generic_ReadyNow()`）；可用 `TranslationClassBuilder_*` MSBuild 属性定制（`ClassName` 默认 `I18n`、`ClassModifiers` 默认 `internal static` 等）。
- **本地参考（唯一引用，不再联网查询该文档）**：`docs/vendor/modtranslationclassbuilder.md`。

## 场景查表

| 场景 | 去查 |
|---|---|
| 定位游戏内类 / 事件触发点 / 数据入口 | 反编译源码（`StardewValley/` 各命名空间） |
| 调 SMAPI（事件、helper、资产） | 本地 SMAPI 源码 `src/SMAPI/*.cs` |
| 写内容包 / 数据格式 | wiki 专题页（在线，从索引页进入） |
| 写 / 审 Harmony patch | Harmony intro + API（镜像 `docs/vendor/harmony-docs/` 或在线） |
| 翻译键 / 文案接入 | 生成器 README（本地） |
| 确认版本 / 迁移事实 | wiki 迁移指南表（镜像索引页内）或运行时 `Game1.version` |

## 维护约定

引用源迁移、新增本地下载或镜像过期时，同步更新本文件与 `docs/vendor/README.md`。