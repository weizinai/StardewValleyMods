# PiCore UI 框架使用说明

> 面向**框架使用者**（含 AI 协作 agent）：本文档自包含，教你用 `PiCore.UI` 搭出游戏内界面。API 签名以代码为准，本文档是对框架的导航与使用约定；两者不一致时以代码为准并请修正本文档。
>
> 适用版本：PiCore **待定（未发布）**——版本号在发布时才定，以 `docs/CHANGELOG.md` 顶部为准。命名空间 `weizinai.StardewValleyMod.PiCore.UI*`（下文简称 `PiCore.UI`）。同目录另见 [配置框架使用约定](config-framework.md)。

## 验证状态说明

「已验证」= 已被实战模组接入且通过游戏内验收（**ActiveMenuAnywhere (AMA)** 之于菜单宿主，**AutoBreakGeode** 之于叠层宿主，**BetterCabin** 之于世界锚定宿主——三者的游戏内验收都与各自模组的验收清单同一次进行）；「未验证」= 已实现、API 形状以代码为准，但尚无模组实际挂载（部分类型虽被宿主内部引用，独立能力未露出）。用未验证类型时，默认它们可用，但如遇布局/交互异常请以源码行为为准回退。

| 状态 | 类型 |
| --- | --- |
| ✅ 已验证 | `MenuHost` · `DrawableHost` · `WorldAnchorHost` · `Element` · `Stack` · `Grid` · `Button` · `Label` · `TabControl` · `Pager` · `IResettable` · `Theme` · `IAnchoredContent` · `TilePanel` |
| ⚠️ 未验证 | `Scrollable` · `Canvas` · `PanelFrame` · `Tooltip` · `FocusManager` · `FocusDirection` · `TextWrap` |

每章开头会标注该章主要类型的验证状态。

---

# 1. 概述

## 1.1 框架定位

PiCore.UI 是一个 **retained 组合式 UI 框架**：你声明一棵**元素树**（`Element` 派生对象），由**宿主**接入游戏绘制与输入管线，框架负责布局、绘制、焦点与交互。它取代了手写 `IClickableMenu` + 每处 `drawTextureBox` + 手算偏移的做法。

- **retained**：树是对象，布局结果缓存，内容变化才重排（`MarkDirty` 标脏），不是每帧从零重建。
- **组合式**：容器套容器，复用 `Stack`/`Grid`/`TabControl`/`Pager` 等成品，不必从底层开始。
- **扁平观感**：SDV 原生形状（九宫格）+ 扁平化（无投影），全部经由 `Theme` 一处收编。

## 1.2 六大块地图

| 块 | 命名空间 | 内容 | 一句话 |
| --- | --- | --- | --- |
| `Host` | `...UI.Host` | `MenuHost` / `DrawableHost` / `WorldAnchorHost` | 接入游戏管线（菜单 / 叠层 / 世界锚定） |
| `Layout` | `...UI.Layout` | `Element` / `LayoutRunner` / `Stack` / `Grid` / `Canvas` / `Scrollable` | 布局内核与容器 |
| `Widget` | `...UI.Widget` | `Button` / `Label` / `PanelFrame` / `TabControl` / `Pager` / `Tooltip` / `TextWrap` / `IResettable` | 可复用控件 |
| `Focus` | `...UI.Focus` | `FocusManager` / `FocusDirection` | 手柄焦点图（几何导航） |
| `World` | `...UI.World` | `IAnchoredContent` / `TilePanel` | 世界锚定只读内容 |
| `Theme` | `...UI` | `Theme` | 扁平 SDV 观感 + 绘制助手 |

另：`PositionHelper`（命名空间 `...PiCore`，**不在** UI 命名空间下）负责世界↔屏幕坐标换算，被 `WorldAnchorHost` 使用。

## 1.3 生命周期

```
1. 建树       —— 构造 Element 派生对象，Add 成树（容器 Add 子级）
2. 挂宿主     —— 交给 MenuHost / DrawableHost / WorldAnchorHost（创建即开始工作）
3. 布局(每帧) —— 宿主在绘制/更新开头调用 LayoutRunner 冲刷挂起的脏布局（Measure→Arrange）
4. 绘制(每帧) —— 宿主调 root.Draw：自绘 + 可见子级递归绘制
5. 交互(每帧) —— MenuHost 路由鼠标/滚轮/手柄（焦点图）；DrawableHost 默认不路由，消费方显式调用时路由鼠标（悬停/左键）
6. 变更       —— 内容变化调 MarkDirty（或 Add/Remove/改 Text 已自动标脏）→ 下一帧重排
```

**核心契约：任何读取元素 `Bounds` 做屏幕定位的代码，必须先经 `LayoutRunner.UpdateIfDirty(...)` 冲刷脏布局**，否则读到的是滚动前/重排前的旧坐标。

## 1.4 术语对照

| 中文 | 英文 | 含义 |
| --- | --- | --- |
| 元素 / 节点 | `Element` | 树中一个节点，有 Bounds、DesiredSize、绘制与子级 |
| 根视图 | `root` | 交给宿主的树根 |
| 测量 / 布置 | `Measure` / `Arrange` | 两趟布局：先算期望尺寸，再定最终矩形 |
| 标脏 | `MarkDirty` / `Dirty` | 内容变化标记，宿主据此决定是否重排 |
| 宿主 | `Host` | 接入游戏管线的容器（Menu/Drawable/WorldAnchor） |
| 获焦项 | `IsFocused` / `FocusManager.Current` | 手柄当前选中项，绘金色环 |
| 内容自适应 | content-adaptive | 按内容期望尺寸生长，不铺满可用区 |
| 内容盒 | `ContentRect` / `InnerViewport` | 边框内缩后的有效区 |

## 1.5 元素树示例

```
MenuHost (根宿主, 直接持有根视图)
└── AMAMenuView : Stack(Vertical)
    ├── Label("Active Menu Anywhere")        标题
    └── TabControl
        ├── [rail] Stack(Horizontal)         页签栏
        │   ├── TabChip : Button("收藏")
        │   ├── TabChip : Button("农场")
        │   └── ...
        └── (当前页签内容, 挂入/移除)
            └── Pager
                ├── (当前页) Grid(3×3)        内容区
                │   └── OptionTile : Element
                │       └── TransparentButton 透明按钮(交互层)
                └── [bar] Stack(Horizontal)   翻页条
                    ├── Button("上一页")
                    ├── Label("1/5")
                    └── Button("下一页")
```

---

# 2. 快速上手（教程）

> 本章主体为 ✅ 已验证路径（以 AMA 的真实用法为蓝本，重写为最小自包含示例）。跟着走完能得到一个带页签 + 分页的可用菜单。

## 2.1 打开一个菜单

最小菜单：一个根视图交给 `MenuHost.OpenMenu` 即可。

```csharp
using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

internal static class DemoMenu
{
    private const int Width = 760;
    private const int Height = 560;

    // 一行调用打开菜单：根视图 + 菜单盒尺寸
    public static void Open()
    {
        MenuHost.OpenMenu(BuildView(), Width, Height);
    }

    // 根视图是一个 Stack(垂直)：标题 + 内容
    private static Element BuildView()
    {
        var view = new Stack(Stack.Direction.Vertical, spacing: 8f);
        view.Add(new Label("我的菜单", center: true));

        var button = new Button("点我");
        button.OnClick = () => Game1.addHUDMessage(new HUDMessage("点击了按钮！"));
        view.Add(button);

        return view;
    }

    // 判断菜单当前是否打开（AMA 的惯用法）
    public static bool IsOpen()
    {
        return Game1.activeClickableMenu is MenuHost menu && menu.Root is not null;
    }
}
```

要点：

- `MenuHost.OpenMenu(root, width, height)` 构造宿主并把 `Game1.activeClickableMenu` 设为它，同时完成首帧布局与焦点图收集。
- 菜单盒默认 760×560；内容布置在 `ContentRect`（外框内缩 24px）内。
- `Button.OnClick` 同时是鼠标左键与手柄 A 的触发（见 §5.2），一次按下只触发一次。

## 2.2 加页签：TabControl

页签把内容分区。每个 `AddTab(title, content)` 加一颗芯片；第一个加入的页签自动选中；切换到实现了 `IResettable` 的内容会调用其 `Reset()`（分页器会回到第一页）。

```csharp
private static Element BuildTabbedView()
{
    var view = new Stack(Stack.Direction.Vertical, spacing: 8f);
    view.Add(new Label("标签页示例", center: true));

    var tabs = new TabControl();
    tabs.AddTab("设置", BuildSettingsContent());   // 返回芯片 Button，可继续设 TooltipText 等
    tabs.AddTab("关于", BuildAboutContent());
    view.Add(tabs);

    return view;
}
```

`TabControl` 布局：顶部横向芯片栏 + 下方内容区占满剩余空间。**注意**：内容固定在芯片栏下方，消费方不应让内容与芯片行在 Y 上重叠（否则破坏左右同排导航）。

## 2.3 加分页：Pager

分页把内容切成多页，底部居中翻页条（上一页 / 页码 / 下一页），首末页自动隐藏对应按钮。页码文案用 `PageLabelFormatter` 定制（框架不含任何语言文案，按钮文字由你设置——参考 AMA 用 `I18n` 本地化）。

```csharp
private static Element BuildPagedContent()
{
    var pager = new Pager(spacing: 8f);
    pager.PreviousButton.Text = "上一页";   // 框架不含文案，请自行本地化
    pager.NextButton.Text = "下一页";

    // 每页一块 3×3 网格，页签一页 9 个
    const int perPage = 9;
    for (var start = 0; start < itemCount; start += perPage)
    {
        var page = new Grid(3, 3, 160f, 160f);
        for (var i = start; i < Math.Min(start + perPage, itemCount); i++)
        {
            page.Add(MakeItemButton(i));
        }
        pager.AddPage(page);   // 链式可返回本 Pager
    }

    return pager;
}
```

要点：

- 一页一个 `Element`（通常是 `Grid`）；内容在上、翻页条在下水平居中。
- 换页 = 换挂内容子级 → 触发树结构版本变化 → 宿主自动重建焦点图并保留仍存在的焦点。
- 需要「切到某页签回到第一页」时：让内容实现 `IResettable`（`Pager` 自己实现了，见 §5.5）。

## 2.4 自定义元素：画自己的交互单元

AMA 的 `OptionTile` 模式：自定义 `Element` 负责视觉，内嵌一个透明 `Button` 负责交互（悬停/点击/焦点环）。这是「自定义外观 + 复用框架交互」的标准姿势。

```csharp
internal class MyTile : Element
{
    // 透明按钮：盒体不绘制，只保留悬停/点击/焦点环
    private sealed class GhostButton : Button
    {
        public GhostButton() : base(string.Empty) { }
        protected override float BoxOpacity => 0f;
    }

    private readonly GhostButton button = new();

    public MyTile(string label, Action onClick)
    {
        this.label = label;
        this.button.OnClick = onClick;
        this.Add(this.button);
    }

    protected override Vector2 MeasureOverride(Vector2 available)
    {
        return new Vector2(160f, 160f);
    }

    protected override void ArrangeOverride(Rectangle final)
    {
        this.button.Arrange(final);   // 交互层铺满本元素
    }

    // 先画子级(交互环/焦点环)，再在之上画自己的视觉
    public override void Draw(SpriteBatch batch)
    {
        if (!this.Visible) return;
        foreach (var child in this.Children)
            if (child.Visible) child.Draw(batch);

        var textSize = Theme.SmallFont.MeasureString(label);
        Theme.DrawPanel(batch, this.Bounds.Center.X - textSize.X / 2 - 16, this.Bounds.Center.Y - 8, (int)textSize.X + 32, (int)textSize.Y + 16);
        Theme.DrawText(batch, label, new Vector2(this.Bounds.Center.X - textSize.X / 2, this.Bounds.Center.Y - textSize.Y / 2));
    }

    private readonly string label;
}
```

要点：

- 自定义元素覆写 `MeasureOverride` / `ArrangeOverride` /（可选）`Draw`；子级交互交给框架。
- `BoxOpacity => 0f` 让按钮盒体透明——交互仍在，只是不画底。
- 绘制顺序：先子级后自身，让交互环被自己的视觉盖住（AMA 里环只在格子边缘露出一圈）。

## 2.5 自定义「切页签重置」内容

实现 `IResettable`：页签被选中时 `TabControl` 自动调用 `Reset()`。AMA 的 `FavoriteTabContent` 借此在切回收藏页签时重建内容。

```csharp
internal class MyResettableContent : Element, IResettable
{
    private Element content = null!;

    public MyResettableContent() => this.Rebuild();

    public void Reset() => this.Rebuild();   // 切到该页签时自动调用

    private void Rebuild()
    {
        if (this.content is not null) this.Remove(this.content);
        this.content = BuildContent();       // 按当前状态重建
        this.Add(this.content);
    }

    protected override Vector2 MeasureOverride(Vector2 available)
    {
        this.content.Measure(available);
        return this.content.DesiredSize;
    }

    protected override void ArrangeOverride(Rectangle final)
    {
        this.content.Arrange(final);
    }
}
```

> 注意：需要保留状态（如滚动位置）的内容**不应**实现 `IResettable`——是否重置由内容自己决定（见 §5.8）。

---

# 3. 宿主层（Host）

> 本章：`MenuHost` / `DrawableHost` / `WorldAnchorHost` ✅ 已验证（分别经 AMA / AutoBreakGeode / BetterCabin）。

## 3.1 MenuHost —— 交互式菜单（✅ 已验证）

`MenuHost : IClickableMenu`。把 retained 根视图装进原生菜单管线，处理全部交互。**大多数情况你用这一个。**

```csharp
// 构造（等价于 OpenMenu 但不设置 Game1.activeClickableMenu）
var menu = new MenuHost(root, width: 760, height: 560);

// 打开（最常用）
MenuHost.OpenMenu(root, width: 760, height: 560);

// 访问
menu.Root;        // 根视图
menu.MenuRect;    // 外框矩形（chrome 覆盖此区域）
menu.ContentRect; // 内容区 = 外框内缩 24px
```

**交互职责（全部由宿主承担，你不用写）**：

- **鼠标**：悬停路由到光标下最上层的可见 `Button`（置 `Hovered`、播悬停音、离开复位）；左键命中最上层按钮并触发 `OnClick` 恰一次；点击后焦点同步到该按钮。
- **滚轮**：滚动光标下最上层的 `Scrollable`。
- **手柄**（`areGamePadControlsImplemented() == true`，SDV 不再把 A 合成左键）：摇杆/方向键按焦点图移焦（初发立刻 + 按住重复，成功移动播导航音、光标落到新获焦项）；A 激活获焦项恰一次；B 关闭；右摇杆滚动焦点所在 `Scrollable`。
- **键盘契约（v1）**：只处理 `Esc` 关闭；其余按键一律忽略且绝不调 base（避免 vanilla 方向键 snap 二次驱动）。
- **提示框**：带 `TooltipText` 的元素在悬停/获焦时显示提示框（见 §5.6）。
- **关闭路径**：右上角 X、`Esc`、手柄 B。鼠标右键不做任何事。

**pad-vs-mouse 消歧**：手柄驱动（摇杆在动/方向键按下/光标非鼠标驱动）时压制鼠标悬停、提示框锚定到获焦项；鼠标一动即交还。

## 3.2 DrawableHost —— 叠层宿主（✅ 已验证）

`DrawableHost` 把同一个 retained 根视图作为**叠层**挂到一条 SMAPI Display 事件（HUD / 活动菜单之后 / 指定渲染步），每帧在 UI 坐标冲刷脏布局后绘制。**默认只读**：宿主自身不订阅任何输入事件（输入所有权留模组侧），鼠标交互由消费方**显式调用**下面两个方法开启——不调用时行为与只读版逐帧一致。

```csharp
using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.UI.Host;

// 根视图按内容尺寸排布在固定左上角 position
var host = DrawableHost.CreateDrawable(root, Helper.Events.Display, DrawableHost.RenderSlot.Hud, new Vector2(16f, 16f));

// 挂到指定渲染步（RenderedStep 每步触发，必须过滤到具体步）
var stepHost = DrawableHost.CreateDrawableOnStep(root, Helper.Events.Display, RenderSteps.World, new Vector2(16f, 16f));

host.Position = new Vector2(32f, 32f);   // 改位置即标脏，下一帧重排
host.Enable();   // 恢复订阅
host.Disable();  // 退订，干净移除（幂等）
host.IsEnabled;  // 当前是否在绘制

// 鼠标交互（可选，消费方显式调用才生效）
host.PerformHoverAction(x, y);                 // 悬停路由：置/复位最上层按钮的悬停态 + 悬停音
var consumed = host.HandleLeftClick(x, y);     // 左键命中：触发 OnClick + 确认音；true = 这一击已归叠层
```

要点：

- `RenderSlot`：`Hud`（RenderedHud 之后）、`ActiveMenu`（RenderedActiveMenu 之后）、`RenderStep`（须给 `CreateDrawableOnStep` 指定步）。
- 放置：内容自适应，向右向下生长；视口边缘夹紧不画出屏幕。
- 叠在活动菜单上（ActiveMenu / RenderStep=Menu）时宿主自动把鼠标光标补画到最上层。
- **交互仅含鼠标**：无焦点图、无手柄导航、无提示框（手柄与键盘路径由消费模组自己的快捷键承担）。何时调用这两个方法由消费方决定——典型接线是把原版每帧的悬停回调转给 `PerformHoverAction`、把左键回调前置给 `HandleLeftClick`。
- `HandleLeftClick` **命中即消费**：返回 true 表示该击属于叠层，消费方应把它吞掉、不再下发给其下的原版菜单（否则叠层按钮与其下重叠的原版点击区会双触发）；未命中返回 false，照常下发。命中但 `OnClick` 为空时不播音，仍算已消费。
- 两个方法都在命中前自动冲刷挂起布局（消费方无需自己调 `LayoutRunner`，首帧绘制之前调用也拿到真实 `Bounds`），且只在启用（`IsEnabled`）时生效：`Disable()` 后不命中、不消费，并复位残留悬停态。

## 3.3 WorldAnchorHost —— 世界锚定叠层（✅ 已验证）

`WorldAnchorHost` 把**只读 immediate 内容**（`IAnchoredContent`）锚定到世界绝对坐标（如玩家），画在 RenderedWorld（世界批坐标空间），垫在 HUD/菜单之下，不参与 retained 布局。

```csharp
using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using weizinai.StardewValleyMod.PiCore.UI.World;

var tilePanel = new TilePanel()
    .AddTitle("矿井入口")
    .AddText("这里有危险！");

var host = WorldAnchorHost.Create(
    Helper.Events.Display,
    worldAnchor: () => PositionHelper.GetAbsolutePositionFromTilePosition(player.Tile),
    tilePanel,
    placement: WorldAnchorHost.AnchorPlacement.Above);

host.Disable();  // 退订（幂等）
host.Enable();   // 恢复
```

要点：

- `worldAnchor` 是**每帧求值**的绝对世界坐标提供者（可为动态对象）；tile 锚点先经 `PositionHelper.GetAbsolutePositionFromTilePosition` 换算。
- `AnchorPlacement`：`Centered`（中心对锚点）/ `Above`（底边对锚点，浮在上方，默认）/ `Below`（顶边对锚点）。
- 整盒与视口求交，完全滚出视口时整帧不绘制。

---

# 4. 布局模型（Layout）

> 本章：`Element` / `Stack` / `Grid` ✅ 已验证（经 AMA）；`Canvas` / `Scrollable` ⚠️ 未验证。

## 4.1 Element —— 布局树节点（✅ 已验证）

`Element` 是**抽象基类**：retained 内核的根视图类型。所有容器与控件都派生自它。

```csharp
public abstract class Element
{
    // 结构
    IReadOnlyList<Element> Children;   // 只读子级视图
    Element? Parent;                   // 父级（未挂时为 null）
    void Add(Element child);           // 加子级并沿父链标脏（抛异常：自引用/已挂他父/成环）
    virtual void Remove(Element child);
    virtual void Clear();

    // 布局结果（Arrange 后有效）
    Rectangle Bounds { get; protected set; }    // 屏幕最终矩形
    Vector2 DesiredSize { get; protected set; } // 上次 Measure 的期望尺寸

    // 状态
    bool Visible { get; set; } = true;   // false = 不绘制不命中；是否跳过布局由容器自定
    object? Tag { get; set; }            // 附加数据，框架不解释
    string? TooltipText { get; set; }    // 提示框文本（宿主绘制）
    virtual bool Focusable => false;     // 是否可获焦（Button 等覆写为 true）
    virtual Action? ActivateAction => null; // 手柄 A 激活动作

    // 布局 / 绘制钩子（子类实现）
    void MarkDirty();                    // 内容变化标脏
    Vector2 Measure(Vector2 available);  // 测量（调 MeasureOverride）
    void Arrange(Rectangle final);       // 布置（调 ArrangeOverride）
    protected abstract Vector2 MeasureOverride(Vector2 available);
    protected abstract void ArrangeOverride(Rectangle final);
    protected virtual void DrawSelf(SpriteBatch batch);  // 自绘，不含子级
    virtual void Draw(SpriteBatch batch);                // 自绘 + 可见子级递归
    virtual bool ContainsPoint(Point point);             // 命中测试（含滚动容器裁剪）
}
```

**两趟布局心智模型**（WPF 式）：

1. `Measure(available)`：把可用约束向下传，每个子级算期望尺寸，向上汇总成 `DesiredSize`。
2. `Arrange(final)`：为每个子级算出最终屏幕矩形，写进 `Bounds`。

内容变化（改 `Text`、`Add`/`Remove`、改尺寸）都会自动 `MarkDirty` 沿父链标脏到根，由宿主每帧统一冲刷。**你一般不需要手动标脏**，除了「改了自己绘制但框架不知道的尺寸/位置」的场景。

## 4.2 LayoutRunner —— 布局冲刷入口（✅ 已验证）

静态类。任何读取 `Bounds` 做屏幕定位的代码，**先冲刷再读**。

```csharp
// 铺满 final 的布局
LayoutRunner.UpdateIfDirty(root, finalRect);

// 内容自适应布局：先用 available 测量取期望尺寸，再交给 place 换算最终矩形
LayoutRunner.UpdateIfDirty(root, availableSize, desired => ComputeRect(desired));

// 无条件强制（首帧 / 内容整体重建）
LayoutRunner.Force(root, finalRect);
LayoutRunner.Force(root, availableSize, placeFunc);
```

- `UpdateIfDirty`：根未标脏则什么都不做（绘制稳定不抖）。
- 宿主（MenuHost/DrawableHost/WorldAnchorHost）每帧自动冲刷，`DrawableHost` 的鼠标输入方法（`PerformHoverAction`/`HandleLeftClick`）在命中前也自行冲刷——**作为消费方你通常不需要直接调**，除非你在事件处理器里改动树后要立即读 `Bounds`。

## 4.3 Stack —— 顺序堆叠容器（✅ 已验证）

沿主轴顺序排布子级（垂直/水平），内容自适应；交叉轴默认撑满，主轴取子级期望尺寸之和。

```csharp
new Stack(Stack.Direction.Vertical, spacing: 8f);
new Stack(Stack.Direction.Horizontal, spacing: 12f);
```

- 子级取自然期望尺寸；容器最终期望尺寸按主轴可用空间封顶，超出部分交给上层裁剪/溢出。
- AMA 的根视图就是 `Stack(Vertical)`（标题 + TabControl）。

## 4.4 Grid —— 固定等大网格（✅ 已验证）

子级排成固定行/列、格子等大的网格（AMA 的 3×3 九宫格）。**整块**（cols×cellW × rows×cellH）在分配区内居中。

```csharp
var grid = new Grid(columns: 3, rows: 3, cellWidth: 160f, cellHeight: 160f);
grid.Add(item0); grid.Add(item1); // ... 按行优先填格子
```

- 格子等大、固定；子级在各自格子内测量/布置。
- 整块居中：分配区比块大时自动水平/垂直居中（HITL 调整）。

## 4.5 Canvas —— 绝对定位容器（⚠️ 未验证）

子级自带位置偏移，按「子级左上角 + 容器左上角」放置。逃逸自动布局的出口。

```csharp
var canvas = new Canvas();
canvas.SetChildPosition(child, new Vector2(100f, 50f));
canvas.GetChildPosition(child);   // 未设置过返回零
canvas.Add(child);
```

- 覆写了 `Remove`/`Clear` 同步清理偏移表。
- 期望尺寸 = 所有子级「偏移 + 尺寸」的外包矩形。

## 4.6 Scrollable —— 可滚动视口（⚠️ 未验证）

固定尺寸的剪裁视口，纵向堆叠子级；滚动偏移在 `Arrange` 时折入子级 `Bounds`（子级得到真实屏幕坐标），绘制时用 scissor 裁在内容区内不溢出。

```csharp
var list = new Scrollable(width: 0f, height: 400f, spacing: 8f);
list.DrawPanel = true;           // 是否画九宫格背景（默认 true）
list.Add(item1); list.Add(item2); // ...

list.ScrollTo(100f);             // 滚动到偏移（裁剪到合法范围）
list.ScrollBy(48f);              // 增量滚动（正值向下）
list.ScrollOffset;               // 当前偏移
list.MaxScrollOffset;            // 最大偏移（内容不满时为 0）
```

要点：

- `width` 传 0 = 不约束、填满可用宽度；非 0 是期望宽度。
- 内容相对九宫格边框内缩 24px（`InnerViewport`），文字/按钮不压边框；绘制与命中测试都裁到内容区，滚出视口的项既不绘制也不可点（不会挡住其上方元素）。
- `MenuHost` 自动把鼠标滚轮 / 手柄右摇杆路由到它；焦点图把获焦项自动滚入视野。

---

# 5. 控件库（Widget）

> 本章：`Button` / `Label` / `TabControl` / `Pager` / `IResettable` ✅ 已验证；`PanelFrame` / `Tooltip` / `TextWrap` ⚠️ 未验证（TextWrap 为 Label/Tooltip 的内部契约，间接经已验证控件生效）。

## 5.1 Label —— 文本标签（✅ 已验证）

单行/多行文本，用游戏语言 SpriteFont 测量与绘制（测得宽 = 绘制宽，扁平无阴影）。支持折行。

```csharp
var label = new Label("Hello", center: false);
label.Text = "新文本";
label.Color = Color.White;
label.Font = Theme.DialogueFont;   // null = Theme.SmallFont
label.MaxWidth = 300f;             // 超出即折行（null = 按布局可用宽）

label.WrappedLines;                // 折出的各行（measure 后有效，单行为 null）
```

**折行规则**（`TextWrap`，§5.7）：CJK 逐字、拉丁按词、收尾标点粘上一行（行首不出现）、`\n` 硬断点。中文无豆腐块的前提是游戏运行在中文语言（字体是语言绑定的）。

## 5.2 Button —— 按钮（✅ 已验证）

九宫格底 + 单行文本，可悬停（宿主控制 `Hovered`）、可点击（`OnClick`）。**可获焦**（`Focusable == true`），手柄 A 的激活 = 鼠标点击（`ActivateAction => OnClick`），一次按下只触发一次。获焦时画金色描边环。

```csharp
var button = new Button("开始");
button.OnClick = () => DoStart();
button.Text = "重命名";
button.Hovered;   // 可读（internal set，宿主置位/复位）
```

可覆写的外观钩子（子类可用，如 AMA 的 `TransparentButton` / TabControl 的 `TabChip`）：

- `protected virtual Color BoxColor` —— 常态白、悬停浅黄。
- `protected virtual float BoxOpacity` —— 盒体透明度（0 = 透明，只留交互环；选中态半透明）。
- `protected virtual Color TextColor` —— 文本颜色（随 BoxColor 可读性）。

## 5.3 PanelFrame —— 九宫格 chrome 面板（⚠️ 未验证）

画一块标准菜单盒九宫格外框，内容在其内侧按 padding 内缩排布。**承载单一内容**——用 `SetContent` 挂入（先清空再加），请勿多次 `Add`（会重叠）。

```csharp
var frame = new PanelFrame(padding: 36f);
frame.SetContent(new Label("面板内容"));
```

## 5.4 TabControl —— 页签容器（✅ 已验证）

顶部横向页签芯片 + 下方内容区。每次选择把旧页签内容从树中移除、把新页签内容挂入（结构版本变化 → 宿主自动重建焦点图并保留仍存在的焦点）；新内容实现 `IResettable` 则挂入后自动 `Reset()`（切到分页内容回到第一页）。选中芯片以半透明盒标示。

```csharp
var tabs = new TabControl(railSpacing: 6f, contentSpacing: 10f);
Button chip = tabs.AddTab("设置", contentA);   // 返回芯片，可继续设 TooltipText 等
tabs.AddTab("关于", contentB);
tabs.Select(1);                 // 选中索引（首个加入的自动选中）
tabs.SelectedIndex;             // 当前选中（-1 = 未选中）
tabs.TabCount;
tabs.SelectedContent;           // 当前选中页签内容（未选中为 null）
```

## 5.5 Pager —— 分页容器（✅ 已验证）

内容区只挂**当前页**（一页一个 `Element`，通常是 `Grid`），底部居中翻页条（上一页/页码/下一页）逐页切换，页码文本自动更新；首末页隐藏对应按钮。实现 `IResettable`：`Reset()` = 回到第一页。

```csharp
var pager = new Pager(spacing: 8f);
pager.PreviousButton.Text = "上一页";   // 按钮点击已接好，设文字/样式即可
pager.NextButton.Text = "下一页";
pager.PageLabelFormatter = (page, count) => $"{page} / {count}";  // 页码文案（null = "当前/总数"）

pager.AddPage(grid1); pager.AddPage(grid2);   // 追加页（首页自动显示）
pager.GoToPage(1);        // 跳到索引页（越界夹紧）
pager.PreviousPage(); pager.NextPage();
pager.ResetToFirstPage(); // 回到第一页
pager.CurrentPage; pager.PageCount; pager.CurrentContent;
```

## 5.6 Tooltip —— 提示框（⚠️ 未验证）

悬停在视图树之上的扁平小面板 + 文本，由宿主排版绘制，**不加入布局树**（宿主持有单个实例反复使用）。**你通常不直接构造 Tooltip**——给元素设 `TooltipText` 即可，宿主自动处理两种摆放：

- 鼠标驱动：跟随光标，鼠标离开即消失（不残留钉住）；视口边缘自动换侧，且**避让获焦项的金色焦点环**。
- 手柄驱动：锚定到获焦项旁边（绝不压住获焦项），跟随焦点移动。

正文按最大内宽（360px）折行，与 `Label` 同一 `TextWrap` 契约。

```csharp
var button = new Button("仓库");
button.TooltipText = "打开仓库界面";   // 就这么简单
```

## 5.7 TextWrap —— 文本折行契约（⚠️ 未验证，被已验证控件间接使用）

共享折行实现，`Label` 与 `Tooltip` 用它，也可直接用：

```csharp
IReadOnlyList<string> lines = TextWrap.Wrap(font, text, maxWidth);
```

## 5.8 IResettable —— 重置缝（✅ 已验证）

实现它的内容在「重新显示/激活」时回到初始状态。`TabControl` 切换页签时对实现它的新内容自动调 `Reset()`；`Pager` 实现它（重置 = 回到第一页）。**需要保留状态的内容不应实现它。**

```csharp
public interface IResettable
{
    void Reset();
}
```

---

# 6. 焦点与手柄（Focus）

> 本章 ⚠️ 未验证：`FocusManager` / `FocusDirection` 无模组直接消费（被 `MenuHost` 内部使用），手柄路径经 `MenuHost` 接通。大多数情况下你**不需要**直接碰它们——`Button`/`TabControl`/`Pager` 的获焦与 A 激活已由宿主自动处理。

## 6.1 FocusDirection（⚠️ 未验证）

```csharp
public enum FocusDirection { Up, Down, Left, Right }
```

## 6.2 FocusManager（⚠️ 未验证）

自建焦点图：从 retained 视图树收集可获焦元素，按**几何规则**移动焦点，取代 vanilla 邻居 ID snap（邻居 ID 不可变，与组合式视图树不兼容）。

```csharp
var focus = new FocusManager(root);   // 收集可获焦元素并置初始焦点（首个）
focus.Current;                        // 当前获焦元素（无可获焦项时 null）
focus.Move(FocusDirection.Down);      // 按几何规则移一格，返回是否移动
focus.Focus(someElement);             // 显式设焦点（须仍在焦点图中）
focus.Activate();                     // 执行当前获焦项的 ActivateAction
focus.RequestRebuild();               // 结构变化时重建（保留当前焦点）
```

**导航规则**（HITL 验证）：按轴向半平面内选目标，距离一律用中心点度量；左右走同排（Y 区间重叠）优先，同排无候选再回退全图；等距时交叉轴偏移最小（向下走同列、向上回同列）。上/下在 `Scrollable` 内先走列表自己的项（自动滚入视口），仅当位于首/末项才离开列表。

## 6.3 谁触发它

- 树结构变化（`Add`/`Remove`/`Clear` 使 `TreeStructureVersion` 变化）→ `MenuHost.update` 自动 `RequestRebuild()`，保留当前焦点（仍存在保留，已移除就近回退）。
- 手柄摇杆/方向键移动、A 激活、B 关闭、右摇杆滚动 —— 全部由 `MenuHost` 内部处理（§3.1）。

---

# 7. 世界锚定（World）

> 本章 ✅ 已验证（经 BetterCabin）：`IAnchoredContent` / `TilePanel`。

## 7.1 IAnchoredContent（✅ 已验证）

世界锚定只读内容契约：`Measure()` 量尺寸、`Draw(batch, bounds)` 在给定屏幕矩形内绘制。只读 immediate：无 retained 布局、无焦点、无输入。消费方也可在任意 RenderedWorld 处理器里直接 `Measure` + `Draw` 自绘。

```csharp
public interface IAnchoredContent
{
    Vector2 Measure();
    void Draw(SpriteBatch batch, Rectangle bounds);
}
```

要点（BetterCabin 实战验证）：宿主用 `Centered` 摆放时，`bounds.Center` 恒等于世界锚点的屏幕位置，因此内容可在盒内相对 `bounds.Center`（= 锚点）自行排布多个子盒——比如 BetterCabin 用一个 `IAnchoredContent` 承载三个 `TilePanel`（名字 / 总在线 / 上次在线），各盒中心 = 锚点 + 各自偏移。两点使用约定：`Measure` 每帧都会被调用，稳态下应返回缓存好的尺寸、避免重复测量文本；返回的尺寸应当是**对锚点对称**的覆盖尺寸（单边最大外扩 × 2），因为宿主是把该尺寸居中摆到锚点上再做整盒视口求交的——不对称的偏移会连带把裁剪框挪偏，边上的子盒就会在真正离开视口前提前消失。

## 7.2 TilePanel（✅ 已验证）

小号世界锚定「面板 + 文本行」盒：标题用 `DialogueFont`、正文用 `SmallFont`，可逐行配色，画成扁平九宫格 + 扁平文字。只负责内容，变换与裁剪由宿主承担。

```csharp
var panel = new TilePanel()
    .AddTitle("标题")          // DialogueFont
    .AddText("正文")           // SmallFont
    .AddLine("自定义行", myFont, Color.Yellow);  // 逐行字体/颜色

panel.Padding = 16f;
panel.LineSpacing = 4f;
```

## 7.3 PositionHelper（坐标换算，位于 `...PiCore`）

世界↔屏幕换算（tile 尺寸 64）。**每次调用读取当前视口**（玩家移动导致视口滚动后不漂移）。常用：

```csharp
PositionHelper.GetAbsolutePositionFromTilePosition(tilePos);       // tile → 绝对世界坐标
PositionHelper.GetScreenPositionFromAbsolutePosition(worldPos);    // 世界 → 屏幕坐标
PositionHelper.GetScreenPositionFromTilePosition(tilePos);
PositionHelper.GetTilePositionFromAbsolutePosition(worldPos);
PositionHelper.GetTilePositionFromScreenPosition(screenPos);
```

---

# 8. 主题（Theme）

> ✅ 已验证（AMA 使用其字体/绘制助手）。

薄主题对象：把 SDV 默认观感（九宫格矩形、关闭按钮矩形、字体、配色、交互音效）与常用绘制助手收编到一处。默认观感 = SDV 原生形状 + 扁平（无投影）。

```csharp
// 常量（只读，直接引用）
Theme.HoverSound;    // "Cowboy_gunshot"  悬停音效名
Theme.NavigateSound; // "toolSwap"        切焦/导航音效名
Theme.AcceptSound;   // "newArtifact"     确认音效名
Theme.CancelSound;   // "cancel"          取消音效名

Theme.MenuBoxSourceRect;  // 标准菜单盒 3×3 九宫格区域
Theme.CloseSourceRect;    // 关闭按钮源区域
Theme.OverlayDim;         // 菜单背景压暗色

// 资源
Theme.MenuTexture;   // => Game1.menuTexture
Theme.MouseCursors;  // => Game1.mouseCursors
Theme.SmallFont;     // => Game1.smallFont（正文）
Theme.DialogueFont;  // => Game1.dialogueFont（对话框）
Theme.TextColor;     // => Game1.textColor

// 绘制助手（扁平：无投影）
Theme.DrawPanel(batch, x, y, width, height, color: null);  // 标准菜单九宫格面板
Theme.DrawText(batch, text, position, color: null, font: null);  // 扁平单行文本
Theme.DrawMouseCursor(batch);  // 光标补画（叠层盖住光标时用）
Theme.PlaySound(Theme.AcceptSound);  // 播游戏内音效
```

---

# 9. 更新约定

本文档是 PiCore.UI 的使用说明书（导航 + 使用约定），源码是唯一事实来源。改动公共 API 时请保持本文档同步：

- **新增/改名/删除** `PiCore.UI` 下任何公共类型或成员 → 同步更新对应章节与文末速查表。
- **行为契约变化**（如布局语义、焦点规则、键盘契约）→ 就地修订相关描述，禁止打补丁式追加。
- 验证状态随实际使用更新：新模组开始使用某类型后，把该类型从 ⚠️ 移到 ✅（同步改文首「验证状态说明」表与文末速查表）。
- changelog（`CHANGELOG.md` / `CHANGELOG.zh.md`）只记用户可见净变化，本文档记使用约定——两条线内容一致但不重复。

---

# 附录：类型速查表

| 类型 | 状态 | 一句话 |
| --- | --- | --- |
| `MenuHost` | ✅ | 交互式菜单宿主，一行 `OpenMenu(root)` 打开，处理全部鼠标/滚轮/手柄/Esc |
| `DrawableHost` | ✅ | 叠层宿主：retained 根视图挂到 HUD/菜单后/渲染步，默认只读；显式调 `PerformHoverAction` / `HandleLeftClick` 才开鼠标悬停与左键消费 |
| `WorldAnchorHost` | ✅ | 只读世界锚定：`IAnchoredContent` 锚到世界坐标画在 RenderedWorld |
| `Element` | ✅ | 抽象基类：两趟布局节点，子类实现 Measure/Arrange/Draw |
| `LayoutRunner` | ✅ | 布局冲刷入口：读 Bounds 前先 `UpdateIfDirty` |
| `Stack` | ✅ | 垂直/水平顺序堆叠，内容自适应 |
| `Grid` | ✅ | 固定等大网格，整块居中 |
| `Canvas` | ⚠️ | 绝对定位容器（子级带偏移） |
| `Scrollable` | ⚠️ | 固定尺寸滚动视口（scissor 裁剪，滚出即不画不可点） |
| `Button` | ✅ | 九宫格按钮：悬停/点击/获焦环，A == 点击 |
| `Label` | ✅ | 文本标签：可折行（CJK 逐字/拉丁按词） |
| `PanelFrame` | ⚠️ | 九宫格 chrome，承载单一内容（`SetContent`） |
| `TabControl` | ✅ | 页签容器：芯片栏 + 内容区，切页签重置 IResettable |
| `Pager` | ✅ | 分页容器：只挂当前页，底部翻页条 |
| `Tooltip` | ⚠️ | 提示框（宿主持有）：设 `TooltipText` 即可，不用自己建 |
| `TextWrap` | ⚠️ | 共享折行契约（Label/Tooltip 用） |
| `IResettable` | ✅ | 重置缝：切页签自动 Reset（需保留状态者勿实现） |
| `FocusManager` | ⚠️ | 几何焦点图（MenuHost 内部使用，通常不用直接碰） |
| `FocusDirection` | ⚠️ | 焦点方向枚举 |
| `IAnchoredContent` | ✅ | 世界锚定内容契约（Measure + Draw） |
| `TilePanel` | ✅ | 世界锚定「面板 + 文本行」盒 |
| `Theme` | ✅ | 扁平 SDV 观感 + 绘制助手/音效 |
| `PositionHelper` | ✅ | 世界↔屏幕坐标换算（位于 `...PiCore`，非 UI 命名空间） |
