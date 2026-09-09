# StardewValleyMods

weizinai 的 Stardew Valley 模组合集（SMAPI，SDV 1.6 / net6.0）：每个顶层目录是一个独立模组，依赖 `PiCore`
（核心库模组，其余模组的运行时依赖）。模组清单、构建步骤、目录结构约定见根 [README.md](README.md)。改动通常只落在一个模组里；动共享层时，要审视所有受影响模组。

## Conventions

**基于证据做决定，不靠猜测。** 遇到不确定的事实，先到可信来源查证再行动：本地优先（游戏/SMAPI 源码树、本仓库文档），然后是官方文档、官方仓库。带确切路径与查阅指引的引用地图见
`docs/agents/modding-references.md`。

**C# 代码风格。** 编写或审查 C# 代码、修改 `.editorconfig` 时，加载 `cs-code-style` 技能并遵循其约定。

**按需构建（构建 = 部署）。** 只构建改动的模组：`dotnet build <改动模组的目录>`（如 `dotnet build AutoBreakGeode`）会把该模组装进游戏 `Mods` 目录，改完直接进游戏验证；引用的共享库（如
PiCore）随引用自动构建。发布 zip 落 `.releases/`。

**版本号与更新日志。** 版本号唯一来源是各 csproj 的 `<Version>`（manifest 用 `%ProjectVersion%` 占位）；模组改动状态由该模组 `docs/CHANGELOG.md`（英文）与
`docs/CHANGELOG.zh.md`（中文）顶部条目判定，两份日志内容始终一致，且只保留未发布的待定条目（历史条目不保留，git 历史即归档；`TestMod` 测试模组与 `[CP] More Size Cabin`
等内容包不维护 changelog）。工作流：功能改动完成后，在两份 changelog 顶部各写一条待定条目 `# [Unreleased] x.y.z`（英文）/ `# [待定] x.y.z`（中文）（含改动列表），并在
csproj `<Version>` 写入同一版本号——此即 **待定**状态；确认发布 N 网后，把两份 changelog 该条目改为 `# x.y.z <日期>`，版本号方为 **定稿**。动工前先读 changelog
顶部判定状态：已有待定条目时，新改动并入该条目且版本号不变；没有时另起新条目，版本号按改动幅度递增（功能改动升中段，修复升末段）。

**日志以旧版本为基准线。** 待定条目描述的是「相对上一已发布版本的净用户可见变化」：只写从旧版本到当前的净效果；周期内临时引入又移除或改名的中间态（如曾被引入又删除的
API）不算变化，不得写入，也不得引用此后已不存在的 API 名；同一待定周期内相互覆盖的条目合并为一条净变化。纯内部实现改动（如方法重载收敛、依赖持有方式、代码文件组织等，对玩家与模组
API 均无感知）同样不算净变化。

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
