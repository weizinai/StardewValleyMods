## Conventions

**基于证据做决定，不靠猜测。** 遇到不确定的事实，先到可信来源查证再行动：本地优先（游戏/SMAPI 源码树、本仓库文档），然后是官方文档、官方仓库。带确切路径与查阅指引的引用地图见 `docs/agents/modding-references.md`。

**C# 代码风格。** 编写或审查 C# 代码、修改 `.editorconfig` 时，加载 `cs-code-style` 技能并遵循其约定。

**Git 提交。** 执行 git 提交时使用 `git-commit` 技能，提交信息用中文。

## Agent skills

### Issue tracker

Issues live as markdown under `.scratch/<feature>/` in this repo. See `docs/agents/issue-tracker.md`.

### Triage labels

Default vocabulary, label string = role name (`needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`). See `docs/agents/triage-labels.md`.

### Domain docs

Single-context: one `CONTEXT.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.