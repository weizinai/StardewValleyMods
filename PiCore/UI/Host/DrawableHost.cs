using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mods;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.PiCore.UI.Host;

/// <summary>
/// Drawable 叠层宿主（票 08）：把同一个 retained 根视图当作**叠层**挂到一条 SMAPI Display
/// 事件（RenderedHud / RenderedActiveMenu / RenderedStep）上，每帧在 UI 坐标冲刷脏布局后绘制。
/// 默认为**只读绘制层**：宿主自身不订阅任何输入事件、不做输入捕获（输入所有权留模组侧），消费方把
/// <see cref="Element" /> 根视图（如 <see cref="Widget.PanelFrame" /> 包一段 <see cref="Widget.Label" />
/// 文本）交进来即可复用菜单同款组件与扁平 <see cref="Theme" /> 绘制助手。
/// 鼠标交互**按消费方显式调用开启**：调 <see cref="PerformHoverAction" /> 得悬停路由（置/复位
/// <see cref="Button.Hovered" /> + 悬停音效），调 <see cref="HandleLeftClick" /> 得左键命中并消费；不调用这两个
/// 方法时行为与只读版逐帧一致（零回归）。交互仅含鼠标——无焦点图/手柄导航/提示框（手柄与键盘路径由消费
/// 模组自身的快捷键承担），且仅在启用（<see cref="IsEnabled" />）时生效：禁用即无绘制，也不参与命中与消费。
/// 放置：根视图在 UI 坐标的固定左上角 <see cref="Position" /> 按**内容尺寸**排布（内容自适应，向右向下
/// 生长，镜像 SMF 固定位置文本盒惯例）；内容变化标脏后下一帧同帧重排，未变化时不重排（绘制稳定不抖）。
/// 视口边缘内容超界时夹紧进视口，不画出屏幕。
/// 启用/禁用：<see cref="CreateDrawable" /> 创建即订阅并开始绘制；<see cref="Disable" /> 退订（干净移除，
/// 无残留绘制），<see cref="Enable" /> 恢复订阅。
/// </summary>
public sealed class DrawableHost
{
    /// <summary>
    /// 可挂接的 SMAPI Display 渲染事件（决定绘制时机/坐标空间）。HUD 与菜单步均在
    /// <see cref="Game1.PushUIMode" /> 内以 UI 坐标绘制，主题绘制助手无需改动即可复用。
    /// </summary>
    public enum RenderSlot
    {
        /// <summary><see cref="IDisplayEvents.RenderedHud" />：HUD 渲染步完成之后（UI 坐标，世界/HUD 可见时）。</summary>
        Hud,

        /// <summary><see cref="IDisplayEvents.RenderedActiveMenu" />：菜单渲染步完成之后（UI 坐标，有活动菜单时）。</summary>
        ActiveMenu,

        /// <summary>
        /// <see cref="IDisplayEvents.RenderedStep" />：指定渲染步完成之后（须给 <see cref="RenderSteps" /> 过滤，
        /// 只在该步完成时绘制，避免 RenderedStep 每步都触发造成重复绘制）。
        /// </summary>
        RenderStep
    }

    private readonly Element root;
    private readonly IDisplayEvents display;
    private readonly RenderSlot slot;
    private readonly RenderSteps? stepFilter;
    private bool enabled;

    /// <summary>当前悬停的按钮（由 <see cref="PerformHoverAction" /> 路由维护，用于复位旧悬停态）。</summary>
    private Button? hoveredButton;

    /// <summary>构造 drawable 叠层宿主并订阅对应 Display 事件（创建即开始绘制）。</summary>
    /// <param name="root">叠层的根视图。</param>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="slot">挂接的渲染事件。</param>
    /// <param name="position">根视图内容盒在 UI 坐标的固定左上角。</param>
    /// <param name="stepFilter"><see cref="RenderSlot.RenderStep" /> 时必填：只在该渲染步完成后绘制。</param>
    /// <exception cref="ArgumentNullException"><paramref name="root" /> 或 <paramref name="display" /> 为 null。</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="slot" /> 为 <see cref="RenderSlot.RenderStep" /> 但未给
    /// <paramref name="stepFilter" />（RenderedStep 每步触发，必须过滤到具体步，否则每帧重复绘制）。
    /// </exception>
    private DrawableHost(Element root, IDisplayEvents display, RenderSlot slot, Vector2 position, RenderSteps? stepFilter)
    {
        this.root = root ?? throw new ArgumentNullException(nameof(root));
        this.display = display ?? throw new ArgumentNullException(nameof(display));
        this.slot = slot;
        this.stepFilter = stepFilter;

        // 与 XML 契约一致：RenderStep 不给定具体渲染步 → 立即失败（RenderedStep 每步触发，无过滤宿主将永不绘制，
        // 静默死宿主比启动即报错更难排查），请改用 CreateDrawableOnStep
        if (slot == RenderSlot.RenderStep && stepFilter is null)
        {
            throw new ArgumentException("RenderSlot.RenderStep 必须给出具体渲染步（RenderedStep 每步触发），请改用 CreateDrawableOnStep。", nameof(slot));
        }

        this.Position = position;
        this.SetSubscribed(true);
    }

    private Vector2 position;

    /// <summary>根视图内容盒在 UI 坐标的固定左上角（改动即标脏根视图，下一帧按新位置重排）。</summary>
    public Vector2 Position
    {
        get => this.position;
        set
        {
            this.position = value;
            this.root.MarkDirty();
        }
    }

    /// <summary>当前是否已订阅事件（启用中）：true = 每帧事件触发时绘制；false = 已退订、无任何绘制。</summary>
    public bool IsEnabled => this.enabled;

    /// <summary>创建 drawable 叠层宿主并开始绘制（镜像 MenuHost.OpenMenu 的一行宿主调用）。</summary>
    /// <param name="root">叠层的根视图。</param>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="slot">挂接的渲染事件。</param>
    /// <param name="position">根视图内容盒在 UI 坐标的固定左上角。</param>
    /// <returns>已启用并开始绘制的宿主实例。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="root" /> 或 <paramref name="display" /> 为 null。</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="slot" /> 为 <see cref="RenderSlot.RenderStep" /> 但未给
    /// </exception>
    public static DrawableHost CreateDrawable(Element root, IDisplayEvents display, RenderSlot slot, Vector2 position)
    {
        return new DrawableHost(root, display, slot, position, stepFilter: null);
    }

    /// <summary>创建挂到指定渲染步的 drawable 叠层宿主（RenderedStep 每步触发，必须过滤到具体步）。</summary>
    /// <param name="root">叠层的根视图。</param>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="step">只在该渲染步完成后绘制的步。</param>
    /// <param name="position">根视图内容盒在 UI 坐标的固定左上角。</param>
    /// <returns>已启用并开始绘制的宿主实例。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="root" /> 或 <paramref name="display" /> 为 null。</exception>
    public static DrawableHost CreateDrawableOnStep(Element root, IDisplayEvents display, RenderSteps step, Vector2 position)
    {
        return new DrawableHost(root, display, RenderSlot.RenderStep, position, step);
    }

    /// <summary>恢复订阅并继续绘制（幂等：已在启用状态则无操作）。</summary>
    public void Enable()
    {
        this.SetSubscribed(true);
    }

    /// <summary>
    /// 退订事件并停止绘制（干净移除，无残留绘制；幂等：已禁用则无操作），并复位残留悬停态——禁用后不再有悬停
    /// 路由，不复位按钮会一直停在悬停视觉上。
    /// </summary>
    public void Disable()
    {
        this.SetSubscribed(false);
        this.SetHovered(null);
    }

    /// <summary>
    /// 悬停路由（消费方显式调用才生效）：把光标下最上层的可见 <see cref="Button" /> 置为悬停态并播放悬停
    /// 音效；悬停空处或换到另一按钮时先复位旧按钮。路由只含鼠标（无手柄消歧——叠层交互仅鼠标，何时调用由
    /// 消费方决定）。宿主禁用时叠层未绘制，故不参与命中，仅复位残留悬停态。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    public void PerformHoverAction(int x, int y)
    {
        if (!this.enabled)
        {
            this.SetHovered(null);

            return;
        }

        this.EnsureLayout();
        this.SetHovered(this.HitTestTopButton(x, y));
    }

    /// <summary>
    /// 左键路由（消费方显式调用才生效）：命中光标下最上层的可见 <see cref="Button" /> 即触发其
    /// <see cref="Button.OnClick" />（非空时播确认音，一次调用触发恰一次），并返回 true 表示这一击已由叠层
    /// **独占消费**——消费方据此把该击吞掉、不再下发给原版菜单，避免叠层按钮与其下重叠的原版点击区双触发。
    /// 未命中返回 false（点击未被消费，消费方照常下发给原版菜单）；宿主禁用时一律返回 false。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>本次点击是否已被叠层消费。</returns>
    public bool HandleLeftClick(int x, int y)
    {
        if (!this.enabled)
        {
            return false;
        }

        this.EnsureLayout();
        var hit = this.HitTestTopButton(x, y);

        if (hit is null)
        {
            return false;
        }

        if (hit.OnClick is not null)
        {
            Theme.PlaySound(Theme.AcceptSound);
            hit.OnClick();
        }

        return true;
    }

    /// <summary>
    /// 冲刷挂起的脏布局（内容变化同帧落地）：绘制与命中测试共用同一处排布入口。命中前必须冲刷——消费方
    /// 可能在首帧绘制之前就调用输入方法，不冲刷会读到未排布的 Bounds（整树零矩形，悬停/点击永不命中）。
    /// </summary>
    private void EnsureLayout()
    {
        Rectangle viewport = new(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        Vector2 available = new(viewport.Width, viewport.Height);

        LayoutRunner.UpdateIfDirty(this.root, available, desired => this.Place(desired, viewport));
    }

    // 下面三个鼠标路由助手与 MenuHost 的鼠标部分形状相同（悬停置/复位 + 最上层命中），但刻意在本宿主内自持一份：
    // 本票的硬契约是「MenuHost 一行不改」（其路由与焦点图/手柄/提示框/关闭按钮纠缠，经 ActiveMenuAnywhere 实战验证），
    // 抽公共助手必然要改动它。两处形状也已分化——本宿主判根节点自身（叠层最小用法 = 直接挂一个 Button 当根视图）、
    // 无焦点同步、无 pad-vs-mouse 消歧、无关闭按钮前置检查；将来若要合并，按分化后的语义重新收敛，而不是复制粘贴。

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

    /// <summary>
    /// 深度优先收集命中点最上层按钮：先本节点、再子级先后，与绘制顺序一致（后绘制的兄弟/后代覆盖先前的
    /// 命中），故取到的是视觉最上层者。本节点自身也参与命中——叠层的最小用法就是把一个 <see cref="Button" />
    /// 直接当根视图交给宿主，不判自身则永远命中不到。
    /// </summary>
    /// <param name="node">当前布局节点。</param>
    /// <param name="point">命中点。</param>
    /// <param name="best">目前命中的按钮（引用传参，整棵树遍历后为最上层者）。</param>
    private void FindTopButton(Element node, Point point, ref Button? best)
    {
        if (node is Button button && button.ContainsPoint(point))
        {
            best = button;
        }

        foreach (var child in node.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            this.FindTopButton(child, point, ref best);
        }
    }

    /// <summary>
    /// 按 <see cref="RenderSlot" /> 订阅或退订对应 Display 事件（单一 switch：订阅与退订共用同一份
    /// slot→事件映射，新增 slot 只改这一处，避免订阅/退订两处各写一份映射导致漂移）。
    /// </summary>
    /// <param name="subscribe">true = 订阅并置启用；false = 退订并停用。</param>
    private void SetSubscribed(bool subscribe)
    {
        if (this.enabled == subscribe)
        {
            return;
        }

        this.enabled = subscribe;

        switch (this.slot)
        {
            case RenderSlot.Hud:
                if (subscribe)
                {
                    this.display.RenderedHud += this.OnRenderedHud;
                }
                else
                {
                    this.display.RenderedHud -= this.OnRenderedHud;
                }

                break;
            case RenderSlot.ActiveMenu:
                if (subscribe)
                {
                    this.display.RenderedActiveMenu += this.OnRenderedActiveMenu;
                }
                else
                {
                    this.display.RenderedActiveMenu -= this.OnRenderedActiveMenu;
                }

                break;
            case RenderSlot.RenderStep:
                if (subscribe)
                {
                    this.display.RenderedStep += this.OnRenderedStep;
                }
                else
                {
                    this.display.RenderedStep -= this.OnRenderedStep;
                }

                break;
        }
    }

    /// <summary>RenderedHud：刷新脏布局后在 UI 坐标绘制根视图。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.Draw(e.SpriteBatch);
    }

    /// <summary>RenderedActiveMenu：刷新脏布局后在 UI 坐标绘制根视图（叠在活动菜单之上）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        this.Draw(e.SpriteBatch);
    }

    /// <summary>RenderedStep：仅在指定渲染步完成时绘制（其余步直接跳过，避免每步重复绘制）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnRenderedStep(object? sender, RenderedStepEventArgs e)
    {
        if (e.Step != this.stepFilter)
        {
            return;
        }

        this.Draw(e.SpriteBatch);
    }

    /// <summary>
    /// 冲刷挂起的脏布局（内容变化同帧落地），再把根视图按内容尺寸排布到固定位置并绘制；
    /// 若绘制点落在活动菜单自身光标之后，叠层画完把鼠标光标补画到最上层（见 <see cref="RedrawCursorAfterActiveMenu" />）。
    /// </summary>
    /// <param name="batch">精灵批（事件触发时已 open，UI 坐标）。</param>
    private void Draw(SpriteBatch batch)
    {
        // 首帧/内容变化后按内容尺寸排布到固定左上角；未变化时不重排（稳定不抖，事件每帧都触发绘制）
        this.EnsureLayout();
        this.root.Draw(batch);

        // 有活动菜单时游戏不代画光标（Game1.drawMouseCursor 仅 activeClickableMenu == null 时画），光标由活动菜单在
        // 自身 draw 末尾画出；本叠层在其后绘制会盖住光标。补画到最上层，保证鼠标始终在叠层上方（与无菜单场景一致）。
        if (this.RedrawCursorAfterActiveMenu)
        {
            Theme.DrawMouseCursor(batch);
        }
    }

    /// <summary>
    /// 当前绘制点是否落在活动菜单自身光标之后（该处须叠层画完补画光标）：RenderedActiveMenu
    /// 与 RenderedStep 过滤到 <see cref="RenderSteps.Menu" /> 都发生在活动菜单 draw（含其末尾光标）之后；其余
    /// 绘制点（HUD/世界等步）由游戏在 DrawOverlays 末尾代画光标、光标天然在本叠层之上，无需补画。
    /// </summary>
    private bool RedrawCursorAfterActiveMenu => this.slot == RenderSlot.ActiveMenu
                                                || this.slot == RenderSlot.RenderStep && this.stepFilter == RenderSteps.Menu;

    /// <summary>把内容期望尺寸换算成最终屏幕矩形：以 <see cref="Position" /> 为左上角，视口边缘超界时夹紧。</summary>
    /// <param name="desired">根视图期望尺寸（内容自适应）。</param>
    /// <param name="viewport">UI 视口。</param>
    /// <returns>最终屏幕矩形。</returns>
    private Rectangle Place(Vector2 desired, Rectangle viewport)
    {
        var width = (int)Math.Min(desired.X, viewport.Width);
        var height = (int)Math.Min(desired.Y, viewport.Height);
        var x = (int)Math.Clamp(this.Position.X, viewport.X, Math.Max(viewport.X, viewport.Right - width));
        var y = (int)Math.Clamp(this.Position.Y, viewport.Y, Math.Max(viewport.Y, viewport.Bottom - height));

        return new Rectangle(x, y, width, height);
    }
}
