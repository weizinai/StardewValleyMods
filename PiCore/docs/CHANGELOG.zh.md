# 更新日志

# [待定] 0.5.0

- 配置模块成员契约为仅公共可写属性：`ConfigService` 重置与 `ConfigMember` 绑定只接受公共 setter 自动属性（配置类必须使用公共自动属性，且永远不会声明索引器），公共字段与索引器成员不参与绑定与重置
- 新增按模组泛型隔离的日志工具（`Logger<T>` / `Broadcaster<T>`），SMAPI 控制台日志保持显示各模组名
- HUD 消息日志独立为 `HudLogger` / `HudBroadcaster`；未初始化的日志调用回退到 PiCore 兜底，不再崩溃
- 多人消息订阅统一由 PiCore `ModEntry` 处理（`MessageData` 置为 public）
- 补丁失败报错按所属模组名显示
- `IPatcher` 新增 `Name` 与 `IsEnabled`（`BasePatcher` 提供默认实现），`HarmonyPatcher` 跳过被禁用的补丁
- 新增单行绑定助手法 `Patch<T>` / `PatchConstructor<T>` 与 `PatchKind` 枚举；`GetHarmonyMethod` 对缺失或非静态补丁方法启动即报错
- 修复 `BaseIntegration<TApi>` 的版本门槛，仅当基类版本检查通过后才拉取 API
- 集成层告警改用消费模组自己的 `IMonitor` 发出，不再走 PiCore 自身的日志器
- GMCM 接口对齐官方 1.16：`IGenericModConfigMenuApi` 替换为官方完整接口（新增 `AddSubHeader` 与 `OpenModMenuAsChildMenu`，`TryGetCurrentMenu` 的 `mod`/`page` 改为可空），GMCM 最低版本上调至 1.16.0；`GenericModConfigMenuIntegration<TConfig>` 同步补全转发成员（`AddSubHeader`/`AddImage`/`AddKeybind`/`AddComplexOption` 等）
- 新增配置模块（`PiCore/Config/`，命名空间 `weizinai.StardewValleyMod.PiCore.Config`）：`ConfigService<TConfig>` 统一负责配置读取（含损坏自愈重置）、GameLaunched 时的 GMCM 注册、保存时写盘，并在保存与重置后各触发一次 `onConfigChanged` 回调；重置把 `new TConfig()` 默认值就地写回当前实例，因此构造期捕获了配置引用的消费者（如 AutoBreakGeode / FriendshipDecayModify 的补丁）无需重启即可看到重置结果。配套声明式菜单描述器 `ConfigMenuDescriptor<TConfig>`（`ConfigMenuSection` 支持嵌套子配置分区），按成员绑定覆盖 bool / int / float / text / enum（带本地化取值显示）/ 按键绑定列表等选项，分区标题（分区开头布尔选项的标签兼作其分区标题）、页面、页内跳转链接与段落，并提供接收原始 `GenericModConfigMenuIntegration<TConfig>` 的 `AddCustomSection` 逃生舱
- 移除各模组自身的 GMCM 注册类，统一经配置模块注册配置菜单，全仓只剩一种写配置菜单的方式
- 修复 `PositionHelper` 在类型加载时采样一次视口的问题；屏幕↔世界坐标换算改为每次调用读取当前视口，玩家移动导致视口滚动后不再漂移