using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI.Host;

namespace weizinai.StardewValleyMod.ReadyCheckKick.UI;

/// <summary>
/// 未准备玩家面板的叠层：持有面板视图与叠层宿主，按调用方给的数据与锚位幂等启停，并把鼠标交互转给宿主。
/// </summary>
/// <remarks>
/// 面板由 <see cref="DrawableHost" /> 挂在活动菜单渲染步上，创建后立刻隐藏——可见性完全由每帧的
/// <c>Sync*</c> 决定，故换菜单、存档结束、关掉配置都不会留下残绘。
/// 位置与文案只在值真正变化时才写入：两者的 setter 都会标脏根视图，每帧无条件写会让布局每帧重跑（绘制抖动）。
/// 叠层宿主默认不订阅任何输入事件：悬停由每帧的同步顺带发起，左键与滚轮由模组侧把 SMAPI 的输入转给
/// <see cref="HandleLeftClick" /> / <see cref="HandleScrollWheel" />。
/// 内部顺序有意固定：启用必须排在悬停之前（宿主的输入方法在禁用时直接返回、不冲刷布局），
/// 测量必须排在按实测尺寸算锚位之前。
/// </remarks>
internal sealed class UnreadyFarmersOverlay
{
    private readonly UnreadyFarmersPanel panel;
    private readonly DrawableHost host;

    /// <summary>构造叠层并订阅活动菜单渲染事件；随即回到隐藏态（只在调用方给非空数据时才可见）。</summary>
    /// <param name="display">消费模组的 Display 事件（<c>helper.Events.Display</c>）。</param>
    /// <param name="actions">交互形态要执行的踢出动作；null = 只读形态（与面板同一处开关）。</param>
    public UnreadyFarmersOverlay(IDisplayEvents display, UnreadyFarmerActions? actions = null)
    {
        this.panel = new UnreadyFarmersPanel(actions);
        this.host = DrawableHost.CreateDrawable(this.panel, display, DrawableHost.RenderSlot.ActiveMenu, Vector2.Zero);
        this.host.Disable();
    }

    /// <summary>
    /// 把面板同步到本帧的事实，锚在固定位置（过夜存盘面：右上角，与迁移前那行红字同位）。
    /// </summary>
    /// <param name="unreadyFarmers">当前未准备玩家；空列表即整块隐藏。</param>
    /// <param name="position">面板左上角在 UI 坐标的固定锚位。</param>
    public void SyncAt(IReadOnlyList<UnreadyFarmer> unreadyFarmers, Vector2 position)
    {
        if (!this.TrySyncContent(unreadyFarmers))
        {
            return;
        }

        this.host.Enable();
        this.RouteHover();
        this.SetPosition(position);
    }

    /// <summary>
    /// 把面板同步到本帧的事实，锚在参考矩形**正上方**（准备检查面：水平居中于原版对话框、底边留固定间隙）。
    /// </summary>
    /// <param name="unreadyFarmers">当前未准备玩家；空列表即整块隐藏。</param>
    /// <param name="reference">参考矩形（原版对话框：取它的水平中线与顶边）；null 即整块隐藏。</param>
    /// <param name="gap">面板底边与参考矩形顶边之间的间隙。</param>
    /// <remarks>
    /// 已知退化形态：面板高度固定（列表视口固定五行），对话框上方的空间比面板还矮时（视口偏矮、或玩家把 UI 缩放调大），
    /// 宿主的夹紧行为会把锚位压回视口顶，面板随之盖住对话框顶部。锚位规则本身没有「挤不下时换位置或收缩视口」的语义——
    /// <see cref="PiCore.UI.Layout.Scrollable" /> 的视口高是构造期只读的，按可用高度改行数要重建列表并丢掉滚动位置，
    /// 故这里只保证「放得下时」底边与对话框顶边的固定间隙。
    /// </remarks>
    public void SyncCenteredAbove(IReadOnlyList<UnreadyFarmer> unreadyFarmers, Rectangle? reference, float gap)
    {
        // 锚位所依附的菜单不在了（对话结束、换成别的菜单）：整块隐藏。不能只是「不写锚位」——面板会留在
        // 上一帧的位置上继续绘制，压住随后出现的界面
        if (reference is null)
        {
            this.host.Disable();

            return;
        }

        if (!this.TrySyncContent(unreadyFarmers))
        {
            return;
        }

        this.host.Enable();
        this.RouteHover();

        // 先测量再定位：锚位按面板的实测尺寸从参考矩形的中线往上反算（面板高由标题与按钮文字高决定）
        var size = this.MeasurePanel();

        this.SetPosition(new Vector2(reference.Value.Center.X - size.X / 2f, reference.Value.Top - gap - size.Y));
    }

    /// <summary>
    /// 把左键交给叠层做命中与点击。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <returns>true = 命中面板按钮并已触发点击（调用方据此**吞掉**这一击，不下发给原版对话框）；未命中或叠层未显示时为 false。</returns>
    public bool HandleLeftClick(int x, int y)
    {
        return this.host.HandleLeftClick(x, y);
    }

    /// <summary>把鼠标滚轮增量交给叠层，滚动光标下最上层的列表（叠层未显示时什么也不做）。</summary>
    /// <param name="delta">SMAPI 滚轮事件的增量（vanilla 符号）。</param>
    public void HandleScrollWheel(int delta)
    {
        this.host.PerformScrollAction(delta);
    }

    /// <summary>
    /// 同步面板内容并在需要时复位滚动。
    /// </summary>
    /// <param name="unreadyFarmers">当前未准备玩家。</param>
    /// <returns>面板是否应当可见（空列表 = 整块隐藏）。</returns>
    private bool TrySyncContent(IReadOnlyList<UnreadyFarmer> unreadyFarmers)
    {
        if (unreadyFarmers.Count == 0)
        {
            this.host.Disable();

            return false;
        }

        // 从隐藏转入显示 = 新的一次等待：把列表拉回顶部（滚动偏移会跨显示残留）
        if (!this.host.IsEnabled)
        {
            this.panel.ResetScroll();
        }

        this.panel.Sync(unreadyFarmers);

        return true;
    }

    /// <summary>悬停路由只在交互形态发起（形态开关见 <see cref="UnreadyFarmersPanel.IsInteractive" />）：只读面板里没有按钮。</summary>
    private void RouteHover()
    {
        if (this.panel.IsInteractive)
        {
            this.host.PerformHoverAction(Game1.getMouseX(), Game1.getMouseY());
        }
    }

    /// <summary>
    /// 面板在当前内容下的实测尺寸。每帧现测而不缓存：锚位要按面板底边反算（先测量后定位），而面板高由标题与
    /// 按钮文字的实测高度决定、随语言与字体浮动，缓存下来会在换语言后留下错位。
    /// </summary>
    /// <returns>面板的期望尺寸（UI 像素）。</returns>
    private Vector2 MeasurePanel()
    {
        return this.panel.Measure(new Vector2(Game1.uiViewport.Width, Game1.uiViewport.Height));
    }

    /// <summary>仅在锚位真正变化时写入：<see cref="DrawableHost.Position" /> 的 setter 会标脏根视图，每帧无条件写会让布局每帧重跑。</summary>
    /// <param name="position">本轮算出的锚位。</param>
    private void SetPosition(Vector2 position)
    {
        if (this.host.Position != position)
        {
            this.host.Position = position;
        }
    }
}
