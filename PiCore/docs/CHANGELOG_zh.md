# 更新日志

# [待定] 0.5.0

- 新增按模组泛型隔离的日志工具（`Logger<T>` / `Broadcaster<T>`）与 monitor 注册表，SMAPI 控制台日志保持显示各模组名
- HUD 消息拆分至非泛型 `HudLogger` / `HudBroadcaster`（日志代码归入 `PiCore/Logging/`）；未初始化的日志调用回退到 PiCore 兜底，不再崩溃
- `MessageData` 置为 public，多人消息订阅统一由 PiCore `ModEntry` 处理
- 补丁失败报错按所属模组名显示

## [0.4.0] 2025-05-09

### 新增内容

- 添加属性访问方法
- 引入字符串常量类以更准确地表示游戏内的物品、怪物和NPC

