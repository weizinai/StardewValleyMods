# 本地镜像（docs/vendor/）

本目录存放从在线源下载的参考快照。**在线规范页仍是事实来源**；快照可能过期，用前先与在线页核对。

下载日期：2026-09-05（本机 web_fetch 工具无外网，改用 pwsh `Invoke-WebRequest` 抓取）。

| 本地文件 | 来源 URL | 内容 |
|---|---|---|
| `stardew-wiki/Modding-Index.html`（+`.txt` 纯文本） | https://stardewvalleywiki.com/Modding:Index | 官方 wiki 模组索引页，修订版 2025-11-15。仅索引页；各专题页在线 |
| `harmony-docs/articles-intro.html`（+`.txt`） | https://harmony.pardeike.net/articles/intro.html | Harmony 2 引言：原理、限制、两种打法 |
| `harmony-docs/api-index.html`（+`.txt`） | https://harmony.pardeike.net/api/index.html | Harmony API 类索引（按 Basics/Targeting/Attributes/Transpiling/Helper 分组） |
| `harmony-docs/api-HarmonyLib.html`（+`.txt`） | https://harmony.pardeike.net/api/HarmonyLib.html | HarmonyLib 命名空间完整类表（含一句话说明） |
| `modtranslationclassbuilder.md` | 快照自 https://github.com/Pathoschild/SMAPI-ModTranslationClassBuilder（raw master README；与 NuGet 包内 README v2.2.0 逐字一致） | 翻译类生成器本地参考：作用、用法、命名约定、定制参数。**只作本地引用，不再联网更新** |

`.txt` 是 `.html` 的纯文本提取（脚本去标签），供直接阅读；`.html` 为原件。新增下载时同步本表。