using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI.World;

namespace weizinai.StardewValleyMod.PiCore.UI.Host;

/// <summary>
/// 世界锚定只读宿主：把只读 immediate 内容（如 <see cref="TilePanel" />）挂到 RenderedWorld 上，
/// 每帧读取世界锚点 → 经 <see cref="PositionHelper" /> 世界→屏幕变换（每次读取当前视口，随视口滚动而追踪）→
/// 显式视口裁剪 → 绘制。与 <see cref="DrawableHost" /> 的 UI 坐标宿主不同：本宿主在世界批坐标空间
/// （世界像素 − 视口）绘制，内容画在世界之上、垫在 HUD/菜单之下，不参与 retained 布局。
/// 锚点：<see cref="Create" /> 的 <c>worldAnchor</c> 参数是每帧求值的绝对世界坐标提供者（可为玩家等
/// 动态对象；tile 锚点先经 <see cref="PositionHelper.GetAbsolutePositionFromTilePosition" /> 换算）。
/// 摆放：内容盒相对屏幕锚点按 <see cref="AnchorPlacement" /> 排布（居中 / 锚点上方 / 锚点下方，可加像素偏移）。
/// 裁剪：整盒屏幕矩形与视口求交，完全在视口外时整帧不绘制（不画出错位内容）。
/// 启用/禁用：创建即订阅并开始绘制；<see cref="Disable" /> 退订（干净移除，无残留绘制），
/// <see cref="Enable" /> 恢复订阅。只读：不订阅任何输入事件（交互世界 UI 不在 v1）。
/// </summary>
public sealed class WorldAnchorHost
{
    /// <summary>内容盒相对世界屏幕锚点的摆放规则。</summary>
    public enum AnchorPlacement
    {
        /// <summary>内容盒中心对准锚点。</summary>
        Centered,

        /// <summary>内容盒水平居中、底边对准锚点（浮在锚点上方）。</summary>
        Above,

        /// <summary>内容盒水平居中、顶边对准锚点（挂在锚点下方）。</summary>
        Below
    }

    private readonly IDisplayEvents display;
    private readonly Func<Vector2> worldAnchor;
    private readonly IAnchoredContent content;
    private readonly AnchorPlacement placement;
    private readonly Vector2 offset;
    private bool enabled;

    /// <summary>构造世界锚定只读宿主并订阅 RenderedWorld（创建即开始绘制）。</summary>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="worldAnchor">每帧求值的世界锚点（绝对世界坐标）。</param>
    /// <param name="content">承载的只读 immediate 内容（测量 + 绘制）。</param>
    /// <param name="placement">内容盒相对屏幕锚点的摆放规则。</param>
    /// <param name="offset">摆放后相对内容盒左上角的像素偏移。</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="display" />、<paramref name="worldAnchor" /> 或
    /// <paramref name="content" /> 为 null。
    /// </exception>
    private WorldAnchorHost(
        IDisplayEvents display,
        Func<Vector2> worldAnchor,
        IAnchoredContent content,
        AnchorPlacement placement,
        Vector2 offset)
    {
        this.display = display ?? throw new ArgumentNullException(nameof(display));
        this.worldAnchor = worldAnchor ?? throw new ArgumentNullException(nameof(worldAnchor));
        this.content = content ?? throw new ArgumentNullException(nameof(content));
        this.placement = placement;
        this.offset = offset;
        this.SetSubscribed(true);
    }

    /// <summary>当前是否已订阅 RenderedWorld（启用中）：true = 每帧世界渲染后绘制；false = 已退订、无任何绘制。</summary>
    public bool IsEnabled => this.enabled;

    /// <summary>创建世界锚定只读宿主并开始绘制（镜像 <see cref="DrawableHost.CreateDrawable" /> 的一行宿主调用）。</summary>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="worldAnchor">
    /// 每帧求值的世界锚点（绝对世界坐标；tile 锚点先经
    /// <see cref="PositionHelper.GetAbsolutePositionFromTilePosition" /> 换算）。
    /// </param>
    /// <param name="content">承载的只读 immediate 内容（测量 + 绘制）。</param>
    /// <param name="placement">内容盒相对屏幕锚点的摆放规则，默认 <see cref="AnchorPlacement.Above" />。</param>
    /// <param name="offset">摆放后相对内容盒左上角的像素偏移，默认 0。</param>
    /// <returns>已启用并开始绘制的宿主实例。</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="display" />、<paramref name="worldAnchor" /> 或
    /// <paramref name="content" /> 为 null。
    /// </exception>
    public static WorldAnchorHost Create(
        IDisplayEvents display,
        Func<Vector2> worldAnchor,
        IAnchoredContent content,
        AnchorPlacement placement = AnchorPlacement.Above,
        Vector2? offset = null)
    {
        return new WorldAnchorHost(display, worldAnchor, content, placement, offset ?? Vector2.Zero);
    }

    /// <summary>恢复订阅并继续绘制（幂等：已在启用状态则无操作）。</summary>
    public void Enable()
    {
        this.SetSubscribed(true);
    }

    /// <summary>退订 RenderedWorld 并停止绘制（干净移除，无残留绘制；幂等：已禁用则无操作）。</summary>
    public void Disable()
    {
        this.SetSubscribed(false);
    }

    /// <summary>订阅或退订 RenderedWorld（单一入口：订阅/退订共用同一事件，避免两处各自写一份漂移）。</summary>
    /// <param name="subscribe">true = 订阅并置启用；false = 退订并停用。</param>
    private void SetSubscribed(bool subscribe)
    {
        if (this.enabled == subscribe)
        {
            return;
        }

        this.enabled = subscribe;

        if (subscribe)
        {
            this.display.RenderedWorld += this.OnRenderedWorld;
        }
        else
        {
            this.display.RenderedWorld -= this.OnRenderedWorld;
        }
    }

    /// <summary>RenderedWorld：世界渲染步完成后绘制世界锚定内容（批已 open，世界批坐标空间）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
    {
        this.Draw(e.SpriteBatch);
    }

    /// <summary>
    /// 读取锚点 → 世界→屏幕变换 → 按摆放规则排布内容盒 → 与视口求交裁剪 → 绘制；
    /// 整盒完全在视口外时直接跳过（不画出错位内容）。
    /// </summary>
    /// <param name="batch">精灵批（世界批坐标空间）。</param>
    private void Draw(SpriteBatch batch)
    {
        var anchorScreen = PositionHelper.GetScreenPositionFromAbsolutePosition(this.worldAnchor());
        var bounds = this.Place(anchorScreen, this.content.Measure());

        // 整盒与视口求交：完全在视口外时跳过（比仅锚点判距更严格——盒的任何部分都不在视口外绘制）
        var viewport = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);

        if (!bounds.Intersects(viewport))
        {
            return;
        }

        this.content.Draw(batch, bounds);
    }

    /// <summary>把屏幕锚点 + 内容尺寸按 <see cref="AnchorPlacement" /> 换算成内容盒的屏幕矩形（含偏移）。</summary>
    /// <param name="anchorScreen">锚点的屏幕坐标（世界像素 − 视口）。</param>
    /// <param name="size">内容尺寸（屏幕像素）。</param>
    /// <returns>内容盒最终屏幕矩形。</returns>
    private Rectangle Place(Vector2 anchorScreen, Vector2 size)
    {
        var sizePoint = size.ToPoint();
        var x = (int)anchorScreen.X - sizePoint.X / 2;

        // y 在 switch 里按摆放规则赋值，须显式 int（var 不允许无初始值声明）
        int y;

        switch (this.placement)
        {
            case AnchorPlacement.Centered:
                y = (int)anchorScreen.Y - sizePoint.Y / 2;

                break;
            case AnchorPlacement.Above:
                y = (int)anchorScreen.Y - sizePoint.Y;

                break;
            case AnchorPlacement.Below:
                y = (int)anchorScreen.Y;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(this.placement));
        }

        return new Rectangle(x + (int)this.offset.X, y + (int)this.offset.Y, sizePoint.X, sizePoint.Y);
    }
}
