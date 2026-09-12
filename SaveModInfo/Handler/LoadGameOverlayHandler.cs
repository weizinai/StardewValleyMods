using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.SaveModInfo.Record;
using weizinai.StardewValleyMod.SaveModInfo.UI;

namespace weizinai.StardewValleyMod.SaveModInfo.Handler;

/// <summary>
/// 存档页叠层的接线：把原版「加载存档」页上的图标与模态窗口这套界面接进事件管线。
/// </summary>
/// <remarks>
/// 本处理器只负责「什么时候把输入交给叠层、以及何时吞掉」，界面本身与菜单身份判断都在
/// <see cref="ModInfoOverlay" /> 里。本模组不再有任何 Harmony 补丁：图标与概览由叠层绘制，
/// 原版存档菜单的 <c>performHoverAction</c> / <c>drawSlotName</c> 一行不改。
/// </remarks>
internal class LoadGameOverlayHandler : BaseHandler
{
    private readonly ModInfoOverlay overlay;

    /// <summary>构造处理器并创建叠层（叠层随即处于隐藏态，等第一帧同步才可能显示）。</summary>
    /// <param name="helper">消费模组的 helper。</param>
    /// <param name="store">记录与差异的存取边界（与记录处理器共用同一实例，缓存失效才能同步）。</param>
    public LoadGameOverlayHandler(IModHelper helper, RecordStore store) : base(helper)
    {
        this.overlay = new ModInfoOverlay(helper.Events.Display, store);
    }

    /// <inheritdoc />
    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        // 叠层宿主默认只读、自身不订阅任何输入事件：交互由本处理器显式转给它，并在它消费后吞掉
        this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        this.helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    /// <summary>每帧把叠层同步到当前事实（按菜单身份启停、更新图块、路由悬停）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据（未用）。</param>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        this.overlay.Sync();
    }

    /// <summary>
    /// 左键与 <c>Esc</c> 的路由。
    /// </summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button == SButton.Escape)
        {
            // 仅在窗口打开时拦截：窗口没开时放行原版行为（原版存档页的 Esc = 回标题菜单）
            if (this.overlay.IsWindowOpen)
            {
                this.overlay.CloseWindow();
                this.helper.Input.Suppress(e.Button);
            }

            return;
        }

        if (e.Button != SButton.MouseLeft)
        {
            return;
        }

        // 图标命中区落在原版存档槽内部：命中即吞，否则点图标会同时开始载入存档
        var consumed = this.overlay.HandleLeftClick(Game1.getMouseX(), Game1.getMouseY());

        // 窗口打开期间吞掉所有左键（含面板空白处）：拦截层铺满视口，命中必为真；这里再兜一道
        if (consumed || this.overlay.IsWindowOpen)
        {
            this.helper.Input.Suppress(e.Button);
        }
    }

    /// <summary>
    /// 滚轮路由：只在模态窗口打开时生效，且吞掉该格（滚轮只滚动窗口内的列表，背后的存档列表不跟着滚）。
    /// </summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据。</param>
    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (!this.overlay.IsWindowOpen)
        {
            return;
        }

        this.overlay.HandleScrollWheel(e.Delta);
        this.helper.Input.SuppressScrollWheel();
    }
}
