# 更新日志

# [待定] 0.5.0

- 新增按模组泛型隔离的日志工具（`Logger<T>` / `Broadcaster<T>`）与 monitor 注册表，SMAPI 控制台日志保持显示各模组名
- HUD 消息拆分至非泛型 `HudLogger` / `HudBroadcaster`（日志代码归入 `PiCore/Logging/`）；未初始化的日志调用回退到 PiCore 兜底，不再崩溃
- `MessageData` 置为 public，多人消息订阅统一由 PiCore `ModEntry` 处理
- 补丁失败报错按所属模组名显示
- `IPatcher` 新增 `Name` 与 `IsEnabled`（`BasePatcher` 提供默认实现）；`HarmonyPatcher` 跳过被禁用的补丁，并记录每个补丁成功与应用汇总日志
- `HarmonyPatcher.Apply` 收敛为单一 `Apply(Mod, ...)` 入口（移除原 `Apply(string, ...)` 重载）
- 新增单行绑定助手法 `Patch<T>` / `PatchConstructor<T>` 与 `PatchKind` 枚举；`GetHarmonyMethod` 对缺失或非静态补丁方法启动即报错
- 补丁依赖改由静态单例实例持有，取代原先拷入静态字段的做法