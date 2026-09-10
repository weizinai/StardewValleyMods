# StardewValleyMods

weizinai 的 Stardew Valley 模组合集（SMAPI，SDV 1.6 / net6.0）：每个顶层目录是一个独立模组，依赖 `PiCore`
（核心库模组，其余模组的运行时依赖）。模组清单、构建步骤、目录结构约定见根 [README.md](README.md)。改动通常只落在一个模组里；动共享层时，要审视所有受影响模组。

## Conventions

**基于证据做决定，不靠猜测。** 遇到不确定的事实，先到可信来源查证再行动：本地优先（游戏/SMAPI 源码树、本仓库文档），然后是官方文档、官方仓库。带确切路径与查阅指引的引用地图见
`docs/agents/modding-references.md`。

**C# 代码风格。** 编写或审查 C# 代码、修改 `.editorconfig` 时，加载 `cs-code-style` 技能并遵循其约定。

**按需构建（构建 = 部署）。** 只构建改动的模组：`dotnet build <改动模组的目录>`（如 `dotnet build AutoBreakGeode`）会把该模组装进游戏 `Mods` 目录，改完直接进游戏验证；引用的共享库（如
PiCore）随引用自动构建。发布 zip 落 `.releases/`。

**版本号与更新日志。** 版本号唯一来源是各 csproj 的 `<Version>`（manifest 用 `%ProjectVersion%` 占位），且**只在发布时改动**：待定期间 csproj 保持上一已发布版本的号不变。模组改动状态由该模组
`docs/CHANGELOG.md`（英文）与 `docs/CHANGELOG.zh.md`（中文）顶部判定，两份内容始终一致（`TestMod` 测试模组与 `[CP] More Size Cabin` 等内容包不维护 changelog）。动工前先读顶部：已有未发布的
待定条目时，新改动并入该条目；没有时另起 `# [Unreleased]`（英文）/ `# [待定]`（中文）。条目按 KaC 六类分节（`Added` / `Changed` / `Deprecated` / `Removed` / `Fixed` / `Security`，中文文件写
新增 / 变更 / 弃用 / 移除 / 修复 / 安全），只写有内容的节；破坏性变更在条目里用文字标明（如 `Breaking:`）。**条目判定与写法加载 `writing-changelogs` 技能并遵循其规则**，本段只记本仓库的形态与例外。发布流程：
在 N 网确认发布后按技能定号，把两份 changelog 的待定条目改为 `# x.y.z <ISO 8601 日期>`（如 `# 1.4.2 2026-02-14`），并把该号写进 csproj `<Version>`；已发布条目**保留**，不再删除。

**配置项改动不算破坏性变更。** 本仓库的破坏性变更基数是玩家与模组 API 的契约，而重命名或移除 `config.json` 里的选项、重置玩家设过的设置**不算**破坏性变更——不升大段，只在条目里用文字
提醒玩家更新后需要重设。升大段留给真正的契约断裂：`PiCore` 的公共 API 破坏性变更，以及模组改名导致新旧版本互不通信这类。

**复用基建优先。** 写代码先复用 PiCore 的 `Patcher`/`Handler`/`Integration`/`Extension`/`Constant`，再自己写；PiCore 是运行时依赖，`ProjectReference` 用
`Private="false"`，manifest `Dependencies` 声明 `weizinai.PiCore` 必装。搭游戏内界面（菜单 / 叠层 / HUD / 世界锚定）复用 PiCore.UI，使用约定见
[PiCore UI 框架使用说明](PiCore/docs/frameworks/ui-framework.md)；改动 `PiCore/UI` 的公共 API 时同步更新该文档与其速查表。

**翻译键位置。** 键定义在 `i18n/default.json`（英文基文案），`zh.json` 是译文；先 `I18n.Init` 再用。

**配置类成员形态。** 会序列化进模组 `config.json` 的类，其成员一律用公共自动属性 `{ get; set; }`（需要默认值时写显式初始化器），不用公共字段；配置菜单经 PiCore
配置框架接入。规则与端到端接入指南见
[PiCore 配置框架使用约定](PiCore/docs/frameworks/config-framework.md)。

## Agent skills

### Issue tracker

Issues live as markdown under `.scratch/<feature>/` in this repo. See `docs/agents/issue-tracker.md`.

### Triage labels

Default vocabulary, label string = role name (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`). See
`docs/agents/triage-labels.md`.

### Domain docs

Single-context: one `CONTEXT.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.
