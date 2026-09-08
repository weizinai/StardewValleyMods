using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.UI.Focus;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.PiCore.UI.Host;

/// <summary>
/// 统一菜单宿主：把 retained 根视图装进原生 <see cref="IClickableMenu" /> 管线（表单 A / 交互式表单 B）。
/// 宿主沿 vanilla 绘制管线自绘：压暗背景 → 扁平 chrome 外框 → 根视图树 → 右上角 X 关闭按钮 → 提示框 →
/// 鼠标最上层，在 <see cref="update" /> 与 <see cref="draw" /> 开头都冲刷挂起的脏布局（先重排再读 Bounds
/// 才得到真实屏幕坐标；draw 内也冲一次，保证点击处理器同帧换挂的内容干净落地）。
/// 鼠标路由：<see cref="performHoverAction" /> 把悬停路由到光标下最上层的可见按钮（置 <see cref="Button.Hovered" />
/// 显示悬停态，进入新按钮时播放 <see cref="Theme.HoverSound" />，离开即复位）；<see cref="receiveLeftClick" />
/// 把左键命中路由到最上层按钮并触发其 <see cref="Button.OnClick" /> 恰一次（右上角 X 关闭后不再触发根视图内按钮，
/// 防叠层双触发），点击后把焦点同步到该按钮（手柄续接从点击处开始）。
/// 手柄路由（自建焦点图，票 03/04）：<see cref="update" /> 直接轮询原始手柄状态（<see cref="Game1.input" />，
/// 不依赖 <c>options.gamepadControls</c>，任意手柄可用）——左摇杆/方向键按焦点图几何规则移动焦点
/// （初发立刻 + 按住重复），成功移动时播导航音并让游戏光标一步落到新获焦项；焦点移出/移入滚动列表视口时
/// 自动滚动把获焦项滚入视野（上/下在列表内优先走本列表的项，首/末项才离开列表）；右摇杆上下滚动焦点所在
/// 的 <see cref="Scrollable" />；A 上升沿激活当前获焦项恰一次；B 上升沿关闭。宿主重写
/// <see cref="areGamePadControlsImplemented" /> → true，SDV 不再把 A 合成一次左键，避免一次按下双触发。
/// pad-vs-mouse 消歧：手柄驱动（摇杆在动/方向键按下/光标非鼠标驱动）时压制鼠标悬停，
/// 鼠标一动（<see cref="Game1.lastCursorMotionWasMouse" /> 恢复）即交还鼠标，互不抢。
/// 滚轮路由：<see cref="receiveScrollWheelAction" /> 滚动光标下最上层的 <see cref="Scrollable" />（vanilla
/// 符号：direction&gt;0 向上滚 → 内容下移 → 偏移减小）。
/// 提示框路由（票 05）：元素（<see cref="Element.TooltipText" />）悬停/获焦时显示扁平提示框，模式按
/// pad-vs-mouse 消歧拆分——鼠标驱动时提示框跟随光标、鼠标离开即消失（不残留钉住），且不盖住焦点环；
/// 手柄驱动时提示框锚定到获焦项旁边（绝不压住获焦项）并跟随焦点移动。提示框是本宿主持有的浮层
/// （<see cref="Widget.Tooltip" />），不加入布局树，绘制在内容与关闭按钮之上、光标之下。
/// 键盘契约（v1）：只处理 Esc 关闭，其余按键一律忽略且绝不调用 base——base 在
/// <c>snappyMenus &amp;&amp; gamepadControls</c> 下会把方向键转成 vanilla snap 移动（合成方向键二次驱动回归源）。
/// 关闭路径仅右上角 X（由 base 左键命中处理）、Esc 与手柄 B；鼠标右键不做任何事（不继承 vanilla 的右键取消习惯）。
/// </summary>
public class MenuHost : IClickableMenu
{
    /// <summary>
    /// 摇杆/方向键的重复触发节奏：方向首次按下立刻移动一次，按住不放先等初始延迟、再按重复间隔连续移动；
    /// 方向变化（含松开重按）时重置节奏。
    /// </summary>
    private sealed class DirectionRepeater
    {
        /// <summary>左摇杆死区：|X|/|Y| 均小于该值视为未推（internal：宿主 <see cref="ReadDirection" /> 也要读）。</summary>
        internal const float StickDeadZone = 0.4f;

        /// <summary>从首次按下到第一次重复的延迟（毫秒）。</summary>
        private const double InitialDelayMs = 300;

        /// <summary>首次重复之后每次重复的间隔（毫秒）。</summary>
        private const double RepeatIntervalMs = 80;

        private TimeSpan? firstHold;
        private TimeSpan? lastTrigger;
        private FocusDirection? heldDirection;

        /// <summary>按当前帧的方向/时间判定是否该移动一次焦点。</summary>
        /// <param name="time">游戏时间。</param>
        /// <param name="direction">当前主方向（null = 未推）。</param>
        /// <returns>应当移动一次时为 true。</returns>
        public bool ShouldMove(GameTime time, FocusDirection? direction)
        {
            if (direction is null)
            {
                this.heldDirection = null;
                this.firstHold = null;
                this.lastTrigger = null;

                return false;
            }

            var now = time.TotalGameTime;

            if (this.heldDirection != direction)
            {
                // 首次按下或方向变化：立刻移动一次并重置节奏
                this.heldDirection = direction;
                this.firstHold = now;
                this.lastTrigger = now;

                return true;
            }

            var holdStart = this.firstHold ?? now;
            var lastMove = this.lastTrigger ?? now;

            if ((now - holdStart).TotalMilliseconds >= InitialDelayMs && (now - lastMove).TotalMilliseconds >= RepeatIntervalMs)
            {
                this.lastTrigger = now;

                return true;
            }

            return false;
        }
    }

    /// <summary>外框相对内容区的内缩（九宫格边框厚度 + 留白），内容不压边框。</summary>
    private const int ContentInset = 24;

    /// <summary>滚轮一格 / 右摇杆一次滚动步进的像素量。</summary>
    private const float ScrollStep = 48f;

    /// <summary>鼠标模式下提示框相对光标的偏移量（像素，向右下偏移，视口边缘自动换侧）。</summary>
    private const int TooltipCursorOffset = 32;

    /// <summary>手柄模式下提示框相对获焦项边框的间距（像素）。</summary>
    private const int TooltipItemGap = 24;

    /// <summary>提示框避让获焦项金色焦点环时向外多出的外扩量（像素，含环本身 5px 再留余量）。</summary>
    private const int TooltipRingAvoid = 10;

    private readonly Element root;
    private readonly FocusManager focus;
    private readonly DirectionRepeater padRepeater = new();
    private readonly DirectionRepeater scrollRepeater = new();
    private readonly Tooltip tooltip = new();

    /// <summary>当前悬停的按钮（鼠标悬停路由在 <see cref="performHoverAction" /> 中维护，用于复位旧悬停态）。</summary>
    private Button? hoveredButton;

    /// <summary>上次见过的本树结构版本号（<see cref="Element.TreeStructureVersion" /> 变化 → 焦点图重建）。</summary>
    private int lastStructureVersion;

    /// <summary>手柄 A 上一帧是否按下（上升沿判定，一次按下只激活一次）。</summary>
    private bool aWasPressed;

    /// <summary>手柄 B 上一帧是否按下（上升沿判定，一次按下只关闭一次）。</summary>
    private bool bWasPressed;

    /// <summary>构造菜单宿主：根视图布置在居中的内容盒内。</summary>
    /// <param name="root">根视图。</param>
    /// <param name="width">菜单盒宽度。</param>
    /// <param name="height">菜单盒高度。</param>
    public MenuHost(Element root, int width = 760, int height = 560)
        : base(
            Game1.uiViewport.Width / 2 - width / 2,
            Game1.uiViewport.Height / 2 - height / 2,
            width,
            height,
            showUpperRightCloseButton: true)
    {
        this.root = root;
        this.MenuRect = new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
        LayoutRunner.Force(root, this.ContentRect); // 首帧前先完成 measure+arrange
        this.focus = new FocusManager(root);        // 焦点图初始收集（置首个可获焦元素为初始焦点）
        this.lastStructureVersion = root.TreeStructureVersion;
    }

    /// <summary>根视图。</summary>
    public Element Root => this.root;

    /// <summary>外框矩形（扁平 chrome 九宫格外框覆盖此区域）。</summary>
    public Rectangle MenuRect { get; }

    /// <summary>内容布局矩形：外框内缩 <see cref="ContentInset" />，根视图布置在此（内容不压边框）。</summary>
    public Rectangle ContentRect => new(
        this.MenuRect.X + ContentInset,
        this.MenuRect.Y + ContentInset,
        Math.Max(1, this.MenuRect.Width - ContentInset * 2),
        Math.Max(1, this.MenuRect.Height - ContentInset * 2));

    /// <summary>打开菜单：构造宿主并置为当前活动菜单（用户故事 10 的一行宿主调用）。</summary>
    /// <param name="root">根视图。</param>
    /// <param name="width">菜单盒宽度。</param>
    /// <param name="height">菜单盒高度。</param>
    /// <returns>已打开的宿主实例（供调用方接线/持有）。</returns>
    public static MenuHost OpenMenu(Element root, int width = 760, int height = 560)
    {
        var menu = new MenuHost(root, width, height);
        Game1.activeClickableMenu = menu;

        return menu;
    }

    /// <summary>vanilla 绘制管线（Game1 已在 UI 坐标空间内 Begin batch）。</summary>
    /// <param name="batch">精灵批。</param>
    public override void draw(SpriteBatch batch)
    {
        // 点击/激活处理器可能在本次绘制前已改动树（如切换页签换挂内容）：绘制前先冲刷挂起的脏布局，
        // 保证首帧就按新内容排版（内容交换干净落地，无残影/错位/闪烁一帧）。
        LayoutRunner.UpdateIfDirty(this.root, this.ContentRect);

        // 1) 压暗背景（尊重“清空背景”可访问性选项；其余时刻菜单盖在半透明黑上）
        if (!Game1.options.showClearBackgrounds)
        {
            batch.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Theme.OverlayDim);
        }

        // 2) 扁平 chrome 外框
        Theme.DrawPanel(batch, this.MenuRect.X, this.MenuRect.Y, this.MenuRect.Width, this.MenuRect.Height);

        // 3) 根视图树（含获焦金色描边环）
        this.root.Draw(batch);

        // 4) 右上角 X 关闭按钮（base.draw 只画 upperRightCloseButton）
        base.draw(batch);

        // 5) 提示框（浮在内容与关闭按钮之上；鼠标跟随 / 手柄锚定，见 DrawTooltip）
        this.DrawTooltip(batch);

        // 6) 鼠标最上层（菜单活动时 Game1 不代画光标，须宿主自画；复用 Theme 的扁平光标助手）
        Theme.DrawMouseCursor(batch);
    }

    /// <summary>每帧冲刷挂起的脏布局、感知结构变化重建焦点图、并处理手柄输入（移焦/光标落点/A/B）。</summary>
    /// <param name="time">游戏时间。</param>
    public override void update(GameTime time)
    {
        base.update(time);
        LayoutRunner.UpdateIfDirty(this.root, this.ContentRect);

        // 结构变化（增/删可获焦元素，仅本菜单树）→ 焦点图重建，保留当前焦点（焦点图内自处理“仍存在保留/移除就近回退”）
        if (this.root.TreeStructureVersion != this.lastStructureVersion)
        {
            this.lastStructureVersion = this.root.TreeStructureVersion;
            this.focus.RequestRebuild();
        }

        this.UpdateGamePad(time);
    }

    /// <summary>
    /// 鼠标悬停路由：把悬停状态路由到光标下最上层的可见按钮。进入新按钮时置其 <see cref="Button.Hovered" />
    /// 并播放悬停音效；离开（悬停空处或换到另一按钮）先复位旧按钮再置新按钮。
    /// pad-vs-mouse 消歧（票 03）：手柄驱动（摇杆在动/方向键按下/光标非鼠标驱动——光标被焦点 snap 移动后
    /// <see cref="Game1.lastCursorMotionWasMouse" /> 为 false）时不当作鼠标悬停，避免手柄驱动时把光标停在哪
    /// 就误认成“鼠标悬停”；鼠标真正一动才交还悬停。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);

        if (IsPadDriven())
        {
            this.SetHovered(null);

            return;
        }

        this.SetHovered(this.HitTestTopButton(x, y));
    }

    /// <summary>
    /// 当前是否手柄驱动（摇杆在动/方向键按下/光标非鼠标驱动——光标被焦点 snap 移动后
    /// <see cref="Game1.lastCursorMotionWasMouse" /> 为 false）。悬停压制与提示框锚定共用同一判定，
    /// 保证“手柄驱动时不抢鼠标悬停”与“提示框锚定到获焦项”行为一致。
    /// </summary>
    private static bool IsPadDriven()
    {
        return Game1.isGamePadThumbstickInMotion() || Game1.isDPadPressed() || !Game1.lastCursorMotionWasMouse;
    }

    /// <summary>
    /// 绘制提示框（票 05）：取当前“带提示文本的悬停/获焦元素”为来源——鼠标驱动时取光标下最上层的带提示
    /// 文本元素（离开即无来源 → 不绘制，不残留钉住）；手柄驱动时取当前获焦项。来源为空或提示文本为空则不绘制。
    /// 两种模式分别摆放：鼠标驱动把提示框放在光标旁（视口边缘自动换侧，且避让获焦项的金色焦点环）；
    /// 手柄驱动把提示框锚定在获焦项旁边（绝不压住获焦项，跟随焦点移动）。提示框绘制于内容与关闭按钮之上、
    /// 光标之下（<see cref="draw" /> 步骤 5）。
    /// </summary>
    /// <param name="batch">精灵批。</param>
    private void DrawTooltip(SpriteBatch batch)
    {
        var source = this.ResolveTooltipSource();

        if (source is null || string.IsNullOrEmpty(source.TooltipText))
        {
            return;
        }

        this.tooltip.Text = source.TooltipText;
        Rectangle viewport = new(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        this.tooltip.Measure(new Vector2(viewport.Width, viewport.Height));

        var placement = IsPadDriven()
            ? this.PlaceTooltipBesideFocused(source.Bounds, viewport)
            : this.PlaceTooltipNearCursor(viewport);
        this.tooltip.Arrange(placement);
        this.tooltip.Draw(batch);
    }

    /// <summary>解析提示框来源：手柄驱动取当前获焦项，鼠标驱动取光标下最上层的带提示文本元素；均无则 null。</summary>
    /// <returns>提示框来源元素；无可显示来源时为 null。</returns>
    private Element? ResolveTooltipSource()
    {
        if (IsPadDriven())
        {
            return this.focus.Current;
        }

        return this.HitTestTopTooltip(Game1.getMouseX(), Game1.getMouseY());
    }

    /// <summary>命中测试：返回光标下最上层的带非空提示文本的可见元素，无则 null。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>带提示文本的命中最上层元素。</returns>
    private Element? HitTestTopTooltip(int x, int y)
    {
        var point = new Point(x, y);
        Element? best = null;
        this.FindTopTooltip(this.root, point, ref best);

        return best;
    }

    /// <summary>深度优先收集命中点最上层带提示文本的元素：遍历顺序与绘制顺序一致，后绘制的兄弟/后代覆盖先前的命中。</summary>
    /// <param name="node">当前布局节点。</param>
    /// <param name="point">命中点。</param>
    /// <param name="best">目前命中的元素（引用传参，整棵树遍历后为最上层者）。</param>
    private void FindTopTooltip(Element node, Point point, ref Element? best)
    {
        foreach (var child in node.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(child.TooltipText) && child.ContainsPoint(point))
            {
                best = child;
            }

            this.FindTopTooltip(child, point, ref best);
        }
    }

    /// <summary>
    /// 鼠标驱动的提示框摆放：以光标为锚点（vanilla drawHoverText 同款换侧——右侧放不下换左侧、下方放不下
    /// 换上方），首选完整落在视口内且不盖住当前获焦项金色焦点环的候选；没有完整落点再夹紧进视口。
    /// 无论光标停在不是获焦项还是停在获焦项自身，都避让获焦项的金色焦点环（票 05 验收：“提示框绝不盖住
    /// 获焦项/焦点环”）——光标在获焦项上时提示框自动换到光标另一侧，而不是压住自己的焦点环。
    /// </summary>
    /// <param name="viewport">UI 视口（屏幕边界）。</param>
    /// <returns>提示框最终矩形。</returns>
    private Rectangle PlaceTooltipNearCursor(Rectangle viewport)
    {
        var width = (int)this.tooltip.DesiredSize.X;
        var height = (int)this.tooltip.DesiredSize.Y;
        var cursor = new Point(Game1.getMouseX(), Game1.getMouseY());

        // 候选：右下 → 左下 → 右上 → 左上（换侧优先级同 vanilla drawHoverText）
        Rectangle[] candidates =
        {
            new(cursor.X + TooltipCursorOffset, cursor.Y + TooltipCursorOffset, width, height),
            new(cursor.X - TooltipCursorOffset - width, cursor.Y + TooltipCursorOffset, width, height),
            new(cursor.X + TooltipCursorOffset, cursor.Y - TooltipCursorOffset - height, width, height),
            new(cursor.X - TooltipCursorOffset - width, cursor.Y - TooltipCursorOffset - height, width, height)
        };

        // 获焦项避让区：存在获焦项时始终启用（提示框绝不压住获焦项或其金色焦点环）
        Rectangle? avoid = this.focus.Current is { } focused ? this.FocusRingZone(focused) : null;

        return PickPlacement(candidates, viewport, avoid, candidates[0]);
    }

    /// <summary>
    /// 手柄驱动的提示框摆放：锚定在获焦项旁边（优先右侧，右放不下换左，再下/上），
    /// 首选完整落在视口内且不压住获焦项/焦点环的候选；没有完整落点再夹紧进视口。
    /// </summary>
    /// <param name="focusedBounds">获焦项屏幕矩形。</param>
    /// <param name="viewport">UI 视口（屏幕边界）。</param>
    /// <returns>提示框最终矩形。</returns>
    private Rectangle PlaceTooltipBesideFocused(Rectangle focusedBounds, Rectangle viewport)
    {
        var width = (int)this.tooltip.DesiredSize.X;
        var height = (int)this.tooltip.DesiredSize.Y;

        // 候选：右侧垂直居中 → 左侧垂直居中 → 下方水平居中 → 上方水平居中
        Rectangle[] candidates =
        {
            new(focusedBounds.Right + TooltipItemGap, focusedBounds.Center.Y - height / 2, width, height),
            new(focusedBounds.Left - TooltipItemGap - width, focusedBounds.Center.Y - height / 2, width, height),
            new(focusedBounds.Center.X - width / 2, focusedBounds.Bottom + TooltipItemGap, width, height),
            new(focusedBounds.Center.X - width / 2, focusedBounds.Top - TooltipItemGap - height, width, height)
        };

        // 极端情况（获焦项充满视口）：退回右下角夹紧，至少可见
        var fallback = new Rectangle(viewport.Right - width, viewport.Top + TooltipItemGap, width, height);

        return PickPlacement(candidates, viewport, focusedBounds, fallback);
    }

    /// <summary>
    /// 从候选里按“完整落点在先、夹紧落点其次、兜底最后”选出提示框位置：第一优先取完整落在视口内且不
    /// 与 <paramref name="blocked" />（获焦项避让区，null = 不避让）相交的候选；没有则取夹紧进视口后仍不与其
    /// 相交的候选；再没有（避让区占满视口等极端情况）退回 <paramref name="fallback" /> 夹紧，至少可见。
    /// 两处摆放（光标旁/获焦项旁）共用同一两趟选择形状，避免复制粘贴。
    /// </summary>
    /// <param name="candidates">候选矩形（按下标顺序即优先级）。</param>
    /// <param name="viewport">UI 视口（屏幕边界）。</param>
    /// <param name="blocked">要避让的矩形（获焦项/焦点环避让区；null = 不避让）。</param>
    /// <param name="fallback">全部候选都不满足时的兜底矩形。</param>
    /// <returns>最终矩形。</returns>
    private static Rectangle PickPlacement(Rectangle[] candidates, Rectangle viewport, Rectangle? blocked, Rectangle fallback)
    {
        foreach (var candidate in candidates)
        {
            if (viewport.Contains(candidate) && NotBlocked(candidate, blocked))
            {
                return candidate;
            }
        }

        foreach (var candidate in candidates)
        {
            var rect = ClampToViewport(candidate, viewport);

            if (NotBlocked(rect, blocked))
            {
                return rect;
            }
        }

        return ClampToViewport(fallback, viewport);
    }

    /// <summary><paramref name="rect" /> 是否不与 <paramref name="blocked" />（null = 不避让）相交。</summary>
    /// <param name="rect">待判定矩形。</param>
    /// <param name="blocked">要避让的矩形；null = 不避让。</param>
    /// <returns>不与避让区相交时为 true。</returns>
    private static bool NotBlocked(Rectangle rect, Rectangle? blocked)
    {
        return blocked is not { } zone || !rect.Intersects(zone);
    }

    /// <summary>获焦项的金色焦点环避让区：获焦项边界外扩（环 + 余量）。</summary>
    /// <param name="focused">获焦元素。</param>
    /// <returns>外扩后的避让矩形。</returns>
    private Rectangle FocusRingZone(Element focused)
    {
        var rect = focused.Bounds;
        rect.Inflate(TooltipRingAvoid, TooltipRingAvoid);

        return rect;
    }

    /// <summary>把矩形夹紧进视口内（坐标超界即平移，尺寸超界即收缩）。</summary>
    /// <param name="rect">待夹紧的矩形。</param>
    /// <param name="viewport">视口。</param>
    /// <returns>夹紧后的矩形。</returns>
    private static Rectangle ClampToViewport(Rectangle rect, Rectangle viewport)
    {
        var width = Math.Min(rect.Width, viewport.Width);
        var height = Math.Min(rect.Height, viewport.Height);
        var x = Math.Clamp(rect.X, viewport.Left, Math.Max(viewport.Left, viewport.Right - width));
        var y = Math.Clamp(rect.Y, viewport.Top, Math.Max(viewport.Top, viewport.Bottom - height));

        return new Rectangle(x, y, width, height);
    }

    /// <summary>切换悬停目标：先复位旧按钮的 <see cref="Button.Hovered" />，再置新按钮并播放悬停音效（进入新按钮时）。</summary>
    /// <param name="button">新悬停按钮（null = 离开按钮悬停空处）。</param>
    private void SetHovered(Button? button)
    {
        if (ReferenceEquals(button, this.hoveredButton))
        {
            return;
        }

        if (this.hoveredButton is not null)
        {
            this.hoveredButton.Hovered = false;
        }

        this.hoveredButton = button;

        if (button is not null)
        {
            button.Hovered = true;
            Theme.PlaySound(Theme.HoverSound);
        }
    }

    /// <summary>
    /// 鼠标左键路由：命中光标下最上层的可见按钮并触发其 <see cref="Button.OnClick" /> 恰一次，点击后把焦点
    /// 同步到该按钮（手柄续接从点击处开始；鼠标明确操作不算“抢焦点”）。
    /// 先走 base 处理右上角 X（点击 X 会关闭菜单）；若 X 已关闭本菜单则不再触发根视图内的按钮，
    /// 避免“点 X 同时点到 X 下重叠按钮”造成叠层双触发。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <param name="playSound">是否播放音效。</param>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);

        // X 已关闭菜单（activeClickableMenu 已不是本菜单）：不再处理根视图里的按钮
        if (Game1.activeClickableMenu != this)
        {
            return;
        }

        var hit = this.HitTestTopButton(x, y);

        if (hit is not null)
        {
            this.focus.Focus(hit);

            if (hit.OnClick is not null)
            {
                Theme.PlaySound(Theme.AcceptSound);
                hit.OnClick();
            }
        }
    }

    /// <summary>命中测试：返回光标下最上层的可见按钮（按绘制顺序最后命中的那个 = 视觉最上层），无则 null。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>命中的按钮。</returns>
    private Button? HitTestTopButton(int x, int y)
    {
        var point = new Point(x, y);
        Button? best = null;
        this.FindTopButton(this.root, point, ref best);

        return best;
    }

    /// <summary>深度优先收集命中点最上层按钮：遍历顺序与绘制顺序一致，后绘制的兄弟/后代覆盖先前的命中（视觉最上层胜出）。</summary>
    /// <param name="node">当前布局节点。</param>
    /// <param name="point">命中点。</param>
    /// <param name="best">目前命中的按钮（引用传参，整棵树遍历后为最上层者）。</param>
    private void FindTopButton(Element node, Point point, ref Button? best)
    {
        foreach (var child in node.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            if (child is Button button && button.ContainsPoint(point))
            {
                best = button;
            }

            this.FindTopButton(child, point, ref best);
        }
    }

    /// <summary>滚轮路由：滚光标下最上层的 <see cref="Scrollable" />（vanilla 符号：direction&gt;0 向上滚 → 内容下移 → 偏移减小）。</summary>
    /// <param name="direction">滚轮增量（相对上一次）。</param>
    public override void receiveScrollWheelAction(int direction)
    {
        if (direction == 0)
        {
            return;
        }

        var scrollable = this.HitTestTopScrollable(Game1.getMouseX(), Game1.getMouseY());

        if (scrollable is null)
        {
            return;
        }

        var notches = Math.Max(1, Math.Abs(direction) / 120);
        scrollable.ScrollBy(-Math.Sign(direction) * notches * ScrollStep);
    }

    /// <summary>命中测试：返回光标下最上层的可见 <see cref="Scrollable" />（绘制顺序最后命中的那个 = 视觉最上层），无则 null。</summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>命中的滚动容器。</returns>
    private Scrollable? HitTestTopScrollable(int x, int y)
    {
        var point = new Point(x, y);
        Scrollable? best = null;
        this.FindTopScrollable(this.root, point, ref best);

        return best;
    }

    /// <summary>深度优先收集命中点最上层滚动容器：后绘制的兄弟/后代覆盖先前的命中；嵌套时内层（后遍历）胜出。</summary>
    /// <param name="node">当前布局节点。</param>
    /// <param name="point">命中点。</param>
    /// <param name="best">目前命中的滚动容器（引用传参，整棵树遍历后为最上层者）。</param>
    private void FindTopScrollable(Element node, Point point, ref Scrollable? best)
    {
        foreach (var child in node.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            if (child is Scrollable scrollable && scrollable.ContainsPoint(point))
            {
                best = scrollable;
            }

            this.FindTopScrollable(child, point, ref best);
        }
    }

    /// <summary>
    /// 键盘契约（v1）：仅 Esc 关闭，其余按键一律忽略，且绝不调用 base。
    /// 不能走 base：base 在 snappyMenus+gamepadControls 下会把方向键转成 applyMovementKey 的 vanilla snap
    /// 移动（= 合成方向键二次驱动回归源），故只精确处理 Keys.Escape。
    /// </summary>
    /// <param name="key">按下的键。</param>
    public override void receiveKeyPress(Keys key)
    {
        if (key == Keys.Escape && this.readyToClose())
        {
            this.exitThisMenu();
        }
    }

    /// <summary>
    /// 本菜单自管手柄输入（A 激活/摇杆移焦/B 关闭），SDV 不得把 A 再合成一次左键，
    /// 避免“手柄 A 触发一次 + SDV 合成点击再触发一次”的双触发（原型验证过的根因）。
    /// </summary>
    public override bool areGamePadControlsImplemented()
    {
        return true;
    }

    /// <summary>
    /// 轮询原始手柄状态处理输入：B 上升沿关闭（单一路径，SDV 把手柄 B 映射成非 Esc 键，不走
    /// <see cref="receiveKeyPress" />）；A 上升沿激活当前获焦项恰一次；左摇杆/方向键按焦点图移动焦点
    /// （初发立刻 + 按住重复），成功移动播导航音并让光标一步落到新获焦项。
    /// </summary>
    /// <param name="time">游戏时间。</param>
    private void UpdateGamePad(GameTime time)
    {
        var pad = Game1.input.GetGamePadState();

        // B 关闭后本帧不再处理其余手柄输入
        if (this.HandleBButton(pad))
        {
            return;
        }

        this.HandleAButton(pad);
        this.HandleFocusMovement(time, pad);
        this.HandleScrollMovement(time, pad);
    }

    /// <summary>B 上升沿关闭菜单（单一路径）；返回是否已触发关闭（是则本帧其余手柄输入不再处理）。</summary>
    /// <param name="pad">手柄状态。</param>
    /// <returns>本帧已触发关闭时为 true。</returns>
    private bool HandleBButton(GamePadState pad)
    {
        var down = pad.Buttons.B == ButtonState.Pressed;

        if (!down)
        {
            this.bWasPressed = false;

            return false;
        }

        if (this.bWasPressed)
        {
            return false; // 已在按下，等释放再判定上升沿
        }

        this.bWasPressed = true;

        if (this.readyToClose())
        {
            this.exitThisMenu();
        }

        return true;
    }

    /// <summary>
    /// A 上升沿激活当前获焦项恰一次（areGamePadControlsImplemented()=true 已声明本菜单自管手柄，
    /// SDV 不会把 A 合成一次左键，激活只走这一条路径）。
    /// </summary>
    /// <param name="pad">手柄状态。</param>
    private void HandleAButton(GamePadState pad)
    {
        var down = pad.Buttons.A == ButtonState.Pressed;

        if (down && !this.aWasPressed)
        {
            this.aWasPressed = true;
            this.ActivateFocused();
        }
        else if (!down)
        {
            this.aWasPressed = false;
        }
    }

    /// <summary>左摇杆/方向键按焦点图移动焦点（初发立刻 + 按住重复）；成功移动播导航音并让光标一步落到新获焦项。</summary>
    /// <param name="time">游戏时间。</param>
    /// <param name="pad">手柄状态。</param>
    private void HandleFocusMovement(GameTime time, GamePadState pad)
    {
        var direction = ReadDirection(pad);

        if (this.padRepeater.ShouldMove(time, direction) && direction is { } dir)
        {
            if (this.focus.Move(dir))
            {
                Theme.PlaySound(Theme.NavigateSound);
                this.SnapCursorToFocused();
            }
        }
    }

    /// <summary>右摇杆上下滚动当前获焦项所在的 <see cref="Scrollable" />（初发立刻 + 按住重复）；只滚动不移动焦点/光标。</summary>
    /// <param name="time">游戏时间。</param>
    /// <param name="pad">手柄状态。</param>
    private void HandleScrollMovement(GameTime time, GamePadState pad)
    {
        var right = pad.ThumbSticks.Right;
        FocusDirection? direction = right.Y > DirectionRepeater.StickDeadZone
            ? FocusDirection.Up
            : right.Y < -DirectionRepeater.StickDeadZone
                ? FocusDirection.Down
                : null;

        if (!this.scrollRepeater.ShouldMove(time, direction) || direction is not { } dir)
        {
            return;
        }

        var scrollable = this.focus.Current is { } focused ? Scrollable.FindAncestor(focused) : null;

        if (scrollable is null)
        {
            return;
        }

        var delta = dir == FocusDirection.Down ? ScrollStep : -ScrollStep;
        scrollable.ScrollBy(delta);
    }

    /// <summary>执行当前获焦元素的激活动作（A 按下时；播放确认音，动作恰一次）。</summary>
    private void ActivateFocused()
    {
        // 无可激活动作不播音；FocusManager.Activate 内部已判空，动作只触发一条路径
        if (this.focus.Current?.ActivateAction is null)
        {
            return;
        }

        Theme.PlaySound(Theme.AcceptSound);
        this.focus.Activate();
    }

    /// <summary>让游戏光标一步落到当前获焦项的中心（复刻 vanilla 手柄菜单“光标跟着选中项”的手感）。</summary>
    private void SnapCursorToFocused()
    {
        if (this.focus.Current is not { } focused)
        {
            return;
        }

        // 读 Bounds 做屏幕定位前先冲刷挂起的脏布局（滚动自动滚入等标脏同帧落地，避免读到滚动前旧坐标）
        LayoutRunner.UpdateIfDirty(this.root, this.ContentRect);
        Game1.setMousePosition(focused.Bounds.Center, ui_scale: true);
    }

    /// <summary>
    /// 从手柄状态读出主方向：方向键优先；左摇杆按主轴判定（|X| 与 |Y| 取较大者，超死区才算），
    /// 避免斜推时主轴判向抖动/误判。
    /// </summary>
    /// <param name="pad">手柄状态。</param>
    /// <returns>主方向；摇杆在死区内/方向键未按时为 null。</returns>
    private static FocusDirection? ReadDirection(GamePadState pad)
    {
        if (pad.DPad.Up == ButtonState.Pressed)
        {
            return FocusDirection.Up;
        }

        if (pad.DPad.Down == ButtonState.Pressed)
        {
            return FocusDirection.Down;
        }

        if (pad.DPad.Left == ButtonState.Pressed)
        {
            return FocusDirection.Left;
        }

        if (pad.DPad.Right == ButtonState.Pressed)
        {
            return FocusDirection.Right;
        }

        var stick = pad.ThumbSticks.Left;

        if (Math.Abs(stick.X) < DirectionRepeater.StickDeadZone && Math.Abs(stick.Y) < DirectionRepeater.StickDeadZone)
        {
            return null;
        }

        if (Math.Abs(stick.X) > Math.Abs(stick.Y))
        {
            return stick.X > 0 ? FocusDirection.Right : FocusDirection.Left;
        }

        return stick.Y > 0 ? FocusDirection.Up : FocusDirection.Down;
    }
}
