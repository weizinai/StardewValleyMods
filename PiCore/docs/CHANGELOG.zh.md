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
- GMCM 接口对齐官方 1.16：`IGenericModConfigMenuApi` 替换为官方完整接口（新增 `AddSubHeader` 与 `OpenModMenuAsChildMenu`，`TryGetCurrentMenu` 的 `mod`/`page`
  改为可空），GMCM 最低版本上调至 1.16.0；`GenericModConfigMenuIntegration<TConfig>` 同步补全转发成员（`AddSubHeader`/`AddImage`/`AddKeybind`/`AddComplexOption` 等）
- 新增配置模块（`PiCore/Config/`，命名空间 `weizinai.StardewValleyMod.PiCore.Config`）：`ConfigService<TConfig>` 统一负责配置读取（含损坏自愈重置）、GameLaunched 时的
  GMCM 注册、保存时写盘，并在保存与重置后各触发一次 `onConfigChanged` 回调；重置把 `new TConfig()` 默认值就地写回当前实例，因此构造期捕获了配置引用的消费者（如
  AutoBreakGeode / FriendshipDecayModify 的补丁）无需重启即可看到重置结果。配套声明式菜单描述器 `ConfigMenuDescriptor<TConfig>`（`ConfigMenuSection`
  支持嵌套子配置分区），按成员绑定覆盖 bool / int / float / text / enum（带本地化取值显示）/ 按键绑定列表等选项，分区标题（分区开头布尔选项的标签兼作其分区标题）、页面、页内跳转链接与段落，并提供接收原始
  `GenericModConfigMenuIntegration<TConfig>` 的 `AddCustomSection` 逃生舱
- 移除各模组自身的 GMCM 注册类，统一经配置模块注册配置菜单，全仓只剩一种写配置菜单的方式
- 修复 `PositionHelper` 在类型加载时采样一次视口的问题；屏幕↔世界坐标换算改为每次调用读取当前视口，玩家移动导致视口滚动后不再漂移
- **UI 框架——地基与菜单宿主：** retained 布局内核位于 `PiCore/UI`（命名空间 `weizinai.StardewValleyMod.PiCore.UI*`）：WPF 式两趟 measure/arrange 的 `Element`
  与脏标定，`LayoutRunner` 在任何读取 Bounds 做屏幕定位前先冲刷挂起的布局；薄扁平 `Theme`（九宫格菜单盒、关闭按钮、字体、调色板、音效名，以及扁平面板/扁平文字/鼠标光标绘制助手）；
  以及 `MenuHost.OpenMenu(root)` 跑原生菜单管线——压暗背景、扁平 chrome、根视图、右上角 X 关闭、鼠标最上层。键盘契约：Esc 关闭（`readyToClose()` →
  `exitThisMenu()`），右键无效，vanilla 方向键 snap 移动保持关闭
- **布局容器与基础控件：** `PiCore/UI/Layout` 下 `Stack`（垂直/水平、内容自适应）、`Grid`（固定格子、整块居中）与 `Canvas`（自由绝对排布，子级各带位置偏移）；
  单行 `Label`（用游戏语言 SpriteFont 测量与绘制，测得宽 = 绘制宽）可设 `MaxWidth` 经共享 `TextWrap` 规则折行——CJK 逐字、拉丁按词、收尾标点粘行、`\n` 硬断点、
  多行高把后续元素推到下方；`Button`（扁平九宫格 + 文本，悬停视觉与音效、`OnClick`）；以及 `PanelFrame` 九宫格 chrome，内缩排布唯一内容。`MenuHost` 把鼠标路由到
  最上层可见按钮：悬停置/复位其状态并播放悬停音效，左键在 X 关闭检查之后触发 `OnClick` 恰一次
- **焦点图与手柄导航：** `FocusManager` / `FocusDirection` 在可获焦控件上构建焦点图，按验证过的几何规则移动焦点（中心点轴向半平面、左右同排优先、等距取最小交叉轴
  偏移），结构变化时重建并保留当前焦点。`MenuHost` 直接轮询手柄：摇杆/方向键移焦（初发 + 按住重复）、A 一次按下激活获焦项恰一次、B 关闭、游戏光标一步落到获焦项
  （鼠标一动交还控制）、手柄输入压制鼠标悬停。`areGamePadControlsImplemented()` 返回 true，SDV 不再为每次 A 合成多余点击；`Element` 暴露 `Focusable` /
  `ActivateAction` / `IsFocused`，`Button` 可获焦并绘制金色获焦环
- **`Scrollable` 可滚动列表：** 固定尺寸、scissor 裁剪的视口，纵向堆叠子级，滚动偏移在 arrange 时折入子级 Bounds。内容相对九宫格边框内缩，绘制与命中测试都裁到
  内部视口——列表项不压在边框上，滚出视口的项既不绘制也不可点（长列表不会挡住其上方元素）。`MenuHost` 以光标上方列表的鼠标滚轮或手柄右摇杆滚动它；焦点图自动把
  获焦项滚入视野（Up/Down 先走列表自身项，仅在其首/末项才离开列表）
- **`Tooltip` 提示框：** 元素的 `TooltipText` 会在内容上方绘制扁平提示框。鼠标驱动的提示框跟随光标、鼠标一离开即消失；手柄驱动的提示框锚定在获焦项旁并跟随焦点。
  摆放避开获焦项与其金色焦点环（光标侧在屏幕边缘自动换到另一侧），正文按最大内宽折行（与 `Label` 同一 `TextWrap` 契约）
- **`TabControl` 页签与 `Pager` 分页：** `TabControl` 持有一横排可选页签芯片 + 内容区——点击芯片或按 A 干净换挂内容（焦点图重建并保留当前芯片），实现
  `IResettable` 的内容（如 `Pager`）切换时重置，选中芯片以半透明盒标示。`Pager` 用上页/下页按钮 + 自动更新页码（首/末页自动隐藏）在页面内容间翻页。`MenuHost`
  在 draw 开头冲刷挂起布局，处理器内换挂的内容同帧干净落地
- **叠层与世界锚定宿主：** `DrawableHost` 把同一套 retained 根视图挂到 RenderedHud、RenderedActiveMenu 或单个渲染步
  （`CreateDrawable(root, display, RenderSlot, position)` / `CreateDrawableOnStep`），默认只读；悬停路由与左键命中为可选项——消费方调用
  `PerformHoverAction(x, y)`（把最上层可见 `Button` 置为悬停态并播悬停音）与 `HandleLeftClick(x, y)`（触发其 `OnClick` 并播确认音，命中即返回 `true`，
  消费方据此把这一击吞掉、不再下发给原版菜单，叠层按钮与其下重叠的原版点击区不再双触发）；
  `WorldAnchorHost.Create(display, worldAnchor, content, placement, offset)` 把内容锚定到世界绝对坐标（如玩家）绘到 RenderedWorld，随实时视口追踪并裁剪
  完全滚出视口的盒子；`TilePanel` 即锚定扁平标题/正文面板（DialogueFont 标题、SmallFont 正文行）。两宿主创建即订阅、按内容尺寸排布根视图、
  `Disable()`/`Enable()` 干净退订/恢复（无残留绘制）；叠在活动菜单上时补画鼠标光标到最上层
