using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mods;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Host;

/// <summary>
/// 只读 drawable 宿主（票 08）：把同一个 retained 根视图当作**只读绘制层**挂到一条 SMAPI Display
/// 事件（RenderedHud / RenderedActiveMenu / RenderedStep）上，每帧在 UI 坐标冲刷脏布局后绘制。
/// 只读：不订阅任何输入事件（无命中、无输入捕获，v1 输入所有权留模组侧），消费方把 <see cref="Element" />
/// 根视图（如 <see cref="Widget.PanelFrame" /> 包一段 <see cref="Widget.Label" /> 文本）交进来即可复用
/// 菜单同款组件与扁平 <see cref="Theme" /> 绘制助手。
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

    /// <summary>构造只读 drawable 宿主并订阅对应 Display 事件（创建即开始绘制）。</summary>
    /// <param name="root">只读绘制层的根视图。</param>
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

    /// <summary>创建只读 drawable 宿主并开始绘制（镜像 MenuHost.OpenMenu 的一行宿主调用）。</summary>
    /// <param name="root">只读绘制层的根视图。</param>
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

    /// <summary>创建挂到指定渲染步的只读 drawable 宿主（RenderedStep 每步触发，必须过滤到具体步）。</summary>
    /// <param name="root">只读绘制层的根视图。</param>
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

    /// <summary>退订事件并停止绘制（干净移除，无残留绘制；幂等：已禁用则无操作）。</summary>
    public void Disable()
    {
        this.SetSubscribed(false);
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
        Rectangle viewport = new(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        Vector2 available = new(viewport.Width, viewport.Height);

        // 首帧/内容变化后按内容尺寸排布到固定左上角；未变化时不重排（稳定不抖，事件每帧都触发绘制）
        LayoutRunner.UpdateIfDirty(this.root, available, desired => this.Place(desired, viewport));
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
