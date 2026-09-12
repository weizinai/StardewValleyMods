# PiCore 日志与提示框架使用说明

> 面向**框架使用者**（模组作者与 AI 协作 agent）：本文档自包含，教你用 `PiCore.Logging` / `PiCore.Hud` / `PiCore.Multiplayer` 把「本机打日志 → 本机显示提示 → 发给其他玩家」跑通。API 签名以代码为准，本文档是对框架的导航与使用约定；两者不一致时以代码为准并请修正本文档。
>
> 适用版本：PiCore **待定（未发布）**——版本号在发布时才定，以 `docs/CHANGELOG.md` 顶部为准。命名空间 `weizinai.StardewValleyMod.PiCore.Logging`（下称 `PiCore.Logging`）、`...PiCore.Hud`（`PiCore.Hud`）、`...PiCore.Multiplayer`（`PiCore.Multiplayer`）。同目录另见 [配置框架使用约定](config-framework.md) 与 [UI 框架使用说明](ui-framework.md)。

## 验证状态说明

「已验证」= 被实战模组接入**且**通过游戏内验收；「未验证」= 已实现且有消费方，但还没做游戏内验收。用未验证类型时默认它们可用，如遇异常以源码行为为准回退（与 [配置框架](config-framework.md)、[UI 框架](ui-framework.md) 同一条判据）。

| 状态 | 类型 |
| --- | --- |
| ✅ 已验证 | `Logger<T>`（7 个模组接入）、`HudLogger`（7 个模组接入） |
| ⚠️ 未验证 | `Broadcaster<T>` · `LogReceiver` · `HudReceiver`——联机两态（客机看到广播日志 / 客机看到广播提示）尚未验收，需要两台客户端 |
| ⚠️ 未验证 | 「未初始化即丢弃」这条语义本身——只有静态论证：改动前全仓 13 处 `Init` 覆盖了全部调用点，未初始化分支不可达；本次不做运行时验证 |

---

# 1. 概述

## 1.1 框架定位

PiCore 日志框架把「说一句话给别人看」拆成**四条通道**，每条通道一个入口，互不借用：

| 通道 | 入口 | 谁看得见 |
| --- | --- | --- |
| 本机控制台 | `Logger<T>` | 本机玩家（SMAPI 控制台来源名 = 本模组） |
| 本机 HUD | `HudLogger` | 本机玩家（屏幕提示） |
| 远端控制台 | `Broadcaster<T>.Info` / `.Alert` | 其他玩家的控制台（来源名 = **发送方**模组） |
| 远端 HUD | `Broadcaster<T>.NoIconHUDMessage` | 其他玩家的屏幕 |

设计目标只有一个：**归属正确**。SMAPI 控制台按 monitor 显示来源名，因此日志必须经各模组自己的 monitor 发出；发送到别人机器上的日志，也要在对端显示为发送方的名字，而不是 PiCore 的名字。配套原则是**宁可丢弃，不可错归属**——认不出归属的消息一律不输出（见 #3）。

## 1.2 组成地图

| 类型 | 命名空间 | 职责 |
| --- | --- | --- |
| `Logger<T>` | `...Logging` | 本机控制台日志：六个等级，经本模组自己的 monitor 发出 |
| `MonitorRegistry` | `...Logging`（internal） | 模组 ID → monitor 注册表：接收侧据它找回**发送方**的 monitor |
| `LogReceiver` | `...Logging`（internal） | 收 `Info` / `Alert` 广播 → 按发送方归属输出；本机没装发送方模组就丢弃 |
| `HudLogger` | `...Hud` | 本机屏幕提示（无图标 / 错误图标），与 monitor 无关 |
| `HudReceiver` | `...Hud`（internal） | 收 `NoIconHudMessage` 广播 → 直接显示，不查任何注册表 |
| `Broadcaster<T>` | `...Multiplayer` | 唯一发送门面：`Info` / `Alert` / `NoIconHUDMessage` |
| `MessageTypes` · `LogMessageData` · `HudMessageData` | `...Multiplayer` | 线协议：消息类型名与载荷（**属性名就是网络字段名**） |
| `ModEntry` | `...PiCore` | 全仓唯一订阅者：把两条接收侧挂到 `ModMessageReceived` |

## 1.3 生命周期

```
1. Entry 初始化 —— 要打本机日志的模组：Logger<ModEntry>.Init(this)
                   要广播的模组：Broadcaster<ModEntry>.Init(this)
2. 本机输出     —— Logger<T>.Info(...) / HudLogger.NoIconHUDMessage(...) 立即生效
3. 远端发送     —— Broadcaster<T>.Xxx(...) 打包成消息，收件人白名单固定为 PiCore 自己
4. 对端接收     —— SMAPI 只把消息投给白名单里出现的模组（即 PiCore），PiCore 再按类型分给两条接收侧
5. 对端输出     —— 日志：查注册表拿发送方 monitor → 按发送方归属输出；查不到就丢弃
                   HUD：直接显示（不要求发送方在对端注册过 monitor）
```

> **核心契约：** `T` 必须是**调用方自己的** `ModEntry` 类型；本机日志必须先 `Logger<ModEntry>.Init(this)`、广播必须先 `Broadcaster<ModEntry>.Init(this)`。未初始化时日志调用被直接丢弃——不会回退到 PiCore 的 monitor，也不会在控制台留下归属不明的行。

## 1.4 术语对照

| 中文 | 英文 | 含义 |
| --- | --- | --- |
| 通道 | channel | 本机/远端 × 控制台/HUD 四条之一 |
| 归属 | attribution | 控制台来源名属于哪个模组，由发出日志的 monitor 决定 |
| 回显 | echo | 广播回到发送者自己那台机器；本框架**不**回显 |
| 白名单 | recipient list | `SendMessage` 的 `modIDs`：对端**接收该消息的模组 ID**，本框架固定写 PiCore |
| 线协议 | wire format | 消息类型名 + 载荷属性名；类名不进网络 |
| 门禁 | gate | 接收侧输出前的检查（日志通道查注册表，HUD 通道不查） |

---

# 2. 快速上手（教程）

## 2.1 本机控制台日志

`ModEntry.cs`：

```csharp
public override void Entry(IModHelper helper)
{
    I18n.Init(helper.Translation);
    Logger<ModEntry>.Init(this);   // 只有要打日志的模组才需要这一行
    // …
}
```

之后在任意位置（Handler / Patcher / UI 都行）：

```csharp
Logger<ModEntry>.Info(I18n.UI_ModLimit_Required(id));
Logger<ModEntry>.Alert(I18n.UI_KickPlayer_ServerTooltip(name));
```

六个等级：`Trace` / `Debug` / `Info` / `Warn` / `Error` / `Alert`。日志经本模组自己的 monitor 发出，因此 SMAPI 控制台与日志文件的来源名就是本模组。**模组侧不需要继承基类或订阅事件**——`Logger<T>` 内部按泛型参数隔离静态状态，`Init` 只负责登记 monitor。

## 2.2 本机 HUD 提示

无需任何初始化：

```csharp
HudLogger.NoIconHUDMessage(I18n.UI_ViewportUnlocked_Tooltip());
HudLogger.ErrorHUDMessage(I18n.UI_LockCabin_Disable());   // 带错误图标
```

第二个参数是显示时长（毫秒，默认 3500）。HUD 与 monitor、与日志初始化**完全无关**：只发 HUD 的模组不需要 `Logger<T>.Init`。

## 2.3 给其他玩家发提示

`ModEntry.cs`：

```csharp
Broadcaster<ModEntry>.Init(this);   // 要广播才需要
```

之后：

```csharp
// 发给除自己以外的所有在线玩家（不传 playerIDs 就是这个语义）
Broadcaster<ModEntry>.NoIconHUDMessage($"{Game1.player.Name}连续3次完美钓鱼");

// 只发给指定玩家
var target = new[] { playerId };
Broadcaster<ModEntry>.Alert(I18n.UI_KickPlayer_ClientTooltip(), target);
Broadcaster<ModEntry>.Info(I18n.UI_ModLimit_Required(id), target);
```

对端玩家看到的效果：控制台日志显示为**你的模组名**（不是 PiCore），HUD 提示直接显示在屏幕上。前提是对端装了 PiCore——白名单里没有 PiCore 的玩家收不到任何东西，这也是**静默**的（不会报错，也不会提示"对方没装"）。

---

# 3. 通道与不变量

## 3.1 各通道的初始化要求

| 通道 | 需要 `Logger<T>.Init` | 需要 `Broadcaster<T>.Init` | 对端需要装 PiCore |
| --- | --- | --- | --- |
| 本机控制台 `Logger<T>` | 是（否则日志被丢弃） | — | — |
| 本机 HUD `HudLogger` | 否 | — | — |
| 远端控制台 `Broadcaster<T>.Info/.Alert` | 否 | 是 | 是 |
| 远端 HUD `Broadcaster<T>.NoIconHUDMessage` | 否 | 是 | 是 |

## 3.2 不变量清单

1. **`T` 是调用方自己的 `ModEntry`。** `Logger<T>` / `Broadcaster<T>` 的静态状态按 `T` 隔离，传别的类型会拿到另一个闭包的 monitor 或 helper。
2. **本机日志必须先 `Logger<ModEntry>.Init(this)`。** 未初始化时的日志调用被丢弃，**不会**归到 PiCore 名下（这是所有通道里唯一一条"静默无害"的失败，见 #4）。
3. **广播必须先 `Broadcaster<ModEntry>.Init(this)`。** 未初始化时调用会抛 `NullReferenceException`（内部 helper 为 `null!`），异常信息里没有"你忘了 Init"这层提示。
4. **广播的收件人白名单必须是 PiCore 自己。** 该常量写在 `Broadcaster<T>` 里（`ReceiverModId`），必须与 `PiCore/manifest.json` 的 `UniqueID` 一致：订阅 `ModMessageReceived` 的是 PiCore，写错会让**所有**广播静默落空。
5. **`playerIDs: null` = 除本机玩家外的所有在线玩家。** 传数组则原样发送（含自己那台，如果数组里有自己）。广播不设计回显——给自己看请走 `Logger<T>` / `HudLogger`。
6. **接收侧门禁不对等。** 日志广播要求**发送方模组在对端机器上也调过 `Init`**（否则对端查不到 monitor，丢弃）；HUD 广播不查注册表，照常显示。
7. **线协议是消息类型名 + 载荷属性名。** 类名不进网络；改属性名会断掉与旧版接收方的互通，改类型名会让旧版接收方直接忽略该消息。
8. **全仓只有 PiCore 订阅 `ModMessageReceived`。** 消费模组不要为同一件事自己订阅：白名单只写 PiCore，别的模组收不到。

---

# 4. 坑

- **忘记 `Logger<T>.Init` 是静默的。** 该模组的日志会消失，控制台既没有这些日志、也没有任何提示——不是崩溃，也不是错归属。排查"我的日志去哪了"时先看 `Entry` 里有没有那一行（仓库骨架：`I18n.Init` 之后、需要打日志时加 `Logger<ModEntry>.Init(this)`）。
- **忘记 `Broadcaster<T>.Init` 是响的，但话说得不清楚。** 抛 `NullReferenceException`，栈顶在 `Broadcaster<T>.Send` 里，看不到"初始化"三个字。
- **白名单写错是静默的，而且是全量的。** 这是历史上真实踩过的坑：白名单曾写成发送方自己的模组 ID，于是三种广播全部落进空气——没有异常、没有日志、没有提示。
- **只发 HUD 的模组不必 `Init`；但只广播控制台日志的模组，对端必须已 `Init`。** 前者不需要 monitor，后者按 #3.2 第 6 条走注册表。
- **单人游戏不广播。** `playerIDs: null` 解析为空数组时 SMAPI 只写一条 verbose 日志后返回，无异常、无用户可见噪音。
- **`Entry` 期间取不到别的模组的 API。** SMAPI 的 `GetApi` 要等所有模组初始化完（`GameLaunched` 之后）才可用，所以本框架是静态类而不是"向 PiCore 要一个 API 对象"——初始化只能靠各模组自己在 `Entry` 里登记（见 #5）。
- **HUD 文案里的时长默认值是 3500 毫秒。** `Broadcaster<T>.NoIconHUDMessage` 与 `HudLogger.NoIconHUDMessage` 各有一份默认值，改动时两处都要看。

---

# 5. 为什么不这么设计（已否决的方案）

以下都是讨论过并**主动放弃**的做法，改动前请先读这一节，避免绕回去：

- **接收侧靠 PiCore 的兜底 monitor 输出。** 曾经的做法：未初始化的模组其日志回退到"首个注册者"（PiCore）的 monitor。后果是把调用方模组的日志记成 PiCore 的，误导排查；现已删除，未初始化即丢弃。
- **白名单写发送方自己的模组 ID。** SMAPI 按"注册该处理器的模组"过滤，而处理器归属 PiCore，因此这种写法让广播静默失效（历史缺陷，已修）。
- **把广播按通道对称拆成两套类型。** 发送只需要"属于本模组的 helper"，与通道无关；接收侧却必须按通道路由。拆开后接收侧仍要按类型分派，只是名义上对称，代价是 HUD 侧泛型化 + 全部调用点改动。
- **让 HUD 接收侧也查 monitor 注册表。** 那会要求"只发 HUD 的模组也得先初始化日志"，把 HUD 与日志生命周期重新绑死。
- **广播回显给发送者。** 发送者本机已有 `Logger<T>` / `HudLogger`，回显会让同一句话出现两遍；需要"给自己看"就走本地通道。
- **给消息类型名加模组前缀。** 官方文档明确类型名无需全局唯一、接收方应检查来源模组 ID；白名单已天然隔离第三方模组。

---

# 6. 更新约定

本文是 PiCore 四条通道（`PiCore/Logging` · `PiCore/Hud` · `PiCore/Multiplayer`）的使用说明书（导航 + 使用约定），源码是唯一事实来源。改动相关公共 API 时请保持本文档同步：

- **新增/改名/删除**三个命名空间下任何公共类型或成员 → 同步更新对应章节与文末速查表。
- **行为契约变化**（如初始化要求、丢弃/门禁语义、`playerIDs` 语义）→ 就地修订相关描述，禁止打补丁式追加。
- **线协议变化**（消息类型名、载荷属性名）→ 必须在本节记一条，并说明它对"新旧版本混装"的影响。
- 验证状态随实际使用更新（⚠️ → ✅ 的判据见文首「验证状态说明」；同步改该处的表与文末速查表）。
- changelog（`CHANGELOG.md` / `CHANGELOG.zh.md`）只记用户可见净变化，本文档记使用约定——两条线内容一致但不重复。

---

# 附录：类型速查表

| 类型 | 状态 | 一句话 |
| --- | --- | --- |
| `Logger<T>` | ✅ | 本机控制台日志，六等级；使用前必须 `Init`，未初始化即丢弃 |
| `Logger<T>.Init(mod)` | ✅ | 绑定本模组 monitor + 登记进注册表；每个要打日志的模组在 `Entry` 里调一次 |
| `HudLogger` | ✅ | 本机 HUD：`NoIconHUDMessage` / `ErrorHUDMessage`，无需初始化 |
| `Broadcaster<T>` | ⚠️ | 远端发送门面：`Info` / `Alert` / `NoIconHUDMessage`；使用前必须 `Init` |
| `Broadcaster<T>.Init(mod)` | ⚠️ | 绑定本模组 helper；要广播的模组在 `Entry` 里调一次 |
| `MessageTypes` | ⚠️ | internal：三个消息类型字符串的唯一定义 |
| `LogMessageData` / `HudMessageData` | ⚠️ | 线协议载荷（`Content` / `TimeLeft` 即网络字段名） |
| `LogReceiver` | ⚠️ | internal：收日志广播 → 按发送方 monitor 输出，查不到即丢弃 |
| `HudReceiver` | ⚠️ | internal：收 HUD 广播 → 直接显示 |
| `MonitorRegistry` | ⚠️ | internal：模组 ID → monitor，供接收侧路由 |
