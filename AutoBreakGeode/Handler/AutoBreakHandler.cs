using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Config;
using weizinai.StardewValleyMod.AutoBreakGeode.Session;
using weizinai.StardewValleyMod.AutoBreakGeode.Speed;
using weizinai.StardewValleyMod.AutoBreakGeode.UI;
using weizinai.StardewValleyMod.AutoBreakGeode.Vanilla;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Handler;

/// <summary>
/// 自动砸的接线与翻译：订阅驱动它的两个事件，把本帧的晶球菜单现场交给 <see cref="AutoBreakSession" /> 决策，
/// 再把会话给出的结论翻译成对原版菜单的动作（<see cref="GeodeMenuContract" />）与本模组叠层的刷新
/// （<see cref="AutoBreakButtonOverlay" />）。
/// </summary>
/// <remarks>
/// 本类不含任何策略：什么时候停由会话说了算，倍率由速度策略说了算，怎么点、怎么补帧、什么时候让行由契约说了算。
/// 开关标志是会话的运行期状态，故它**不进**配置变更重建流程（模组入口只 <c>Apply</c> 一次、不重建）：
/// 本模组没有任何按配置构建的东西，速度选项在循环里现读、快捷键在事件里现查，重建只会白丢一次开关状态。
/// </remarks>
internal class AutoBreakHandler : BaseHandler
{
    private readonly AutoBreakSession session = new();
    private readonly GeodeSpeedPolicy speedPolicy;
    private readonly AutoBreakButtonOverlay beginButtonOverlay;

    /// <summary>构造处理器：建好开始按钮叠层，并持有模组入口装配好的速度策略。</summary>
    /// <param name="helper">模组的事件与输入入口。</param>
    /// <param name="speedPolicy">动画速度归属策略，与配置菜单共用同一个实例。</param>
    public AutoBreakHandler(IModHelper helper, GeodeSpeedPolicy speedPolicy) : base(helper)
    {
        this.speedPolicy = speedPolicy;
        this.beginButtonOverlay = new AutoBreakButtonOverlay(helper.Events.Display, this.ToggleAutoBreaking);
    }

    /// <inheritdoc />
    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    /// <inheritdoc />
    /// <remarks>
    /// 目前跑不到：模组入口只 <c>Apply</c> 一次、刻意不进配置变更重建流程（理由见类注释），故没有任何调用方。
    /// 仍然保留是因为退订是本处理器对 <see cref="IHandler" /> 的完整交代——将来若接进重建流程，缺了它就会静默残留订阅。
    /// </remarks>
    public override void Clear()
    {
        this.helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
        this.helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // 菜单身份只有一个判据（GeodeContext 里的那一次转型），快捷键分支与按钮分支都用它：不在此处另写一次转型
        if (GeodeContext.Capture().Menu is null) return;

        if (e.Button == SButton.MouseLeft)
        {
            // 命中即吞：叠层的 HandleLeftClick 返回 true 表示这一击已由叠层独占消费，必须吞掉它，不能再下发给原版菜单。
            // 不能省：视口过窄时按钮会被宿主夹进晶球点击区，不吞就会一次点击同时翻转开关并砸开一颗晶球
            // （夹取机制与面板尺寸见 AutoBreakButtonOverlay 的类注释）
            if (this.beginButtonOverlay.HandleLeftClick(Game1.getMouseX(), Game1.getMouseY()))
            {
                this.helper.Input.Suppress(e.Button);
            }
        }

        if (ModConfig.Instance.ToggleAutoBreakKeybind.JustPressed())
        {
            this.ToggleAutoBreaking();
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        // 菜单现场既是叠层的可见性来源、也是自动砸的上下文来源：下面这一次捕获同时供两者使用
        var context = GeodeContext.Capture();
        // 让行事实只在真的用得上时求值（开关开着、且晶球菜单在），与重构前的短路一致：契约的判据读 Game1.options，
        // 其注释里的守卫以「晶球菜单已开」为前提
        var shouldYieldToPause = this.session.IsAutoBreaking && context.Menu is not null && GeodeMenuContract.ShouldYieldToGamePause();
        var action = this.session.Update(context, shouldYieldToPause);

        if (context.Menu is { } geodeMenu)
        {
            this.ApplyAction(action, geodeMenu);
        }

        // 一次刷新把可见性、文案（由会话状态直接推导）、悬停与锚位同步到本帧，且置于自动砸之后：
        // 自动停止（无晶球 / 钱不够 / 背包满）与快捷键切换都在同一帧翻正，不闪烁；其中的先后由叠层自己定
        this.beginButtonOverlay.Sync(context.Menu, this.session.IsAutoBreaking);
    }

    /// <summary>按钮的点击入口：把这一击交给会话的开关切换，语义与守卫都定义在 <see cref="AutoBreakSession.Toggle" />。</summary>
    /// <remarks>现场现捕：按钮的 <c>OnClick</c> 只有 <see cref="System.Action" /> 一种形状、拿不到事件里那份现场，故与快捷键分支各捕一份同一帧的现场。</remarks>
    private void ToggleAutoBreaking()
    {
        this.session.Toggle(GeodeContext.Capture());
    }

    /// <summary>把会话给出的本帧结论翻译成对原版菜单的动作。</summary>
    /// <param name="action">会话给出的本帧结论。</param>
    /// <param name="geodeMenu">本帧的晶球菜单（结论不是 <see cref="AutoBreakAction.None" /> 时必然存在）。</param>
    private void ApplyAction(AutoBreakAction action, GeodeMenu geodeMenu)
    {
        switch (action)
        {
            case AutoBreakAction.SpeedUp:
                // 倍率由速度策略现取、且已夹取好：速度归别的模组时它恒为 1，契约里的循环自己就不执行
                GeodeMenuContract.AdvanceCrackingAnimation(geodeMenu, this.speedPolicy.SpeedUpTimes);

                break;
            case AutoBreakAction.Crack:
                // 原版没开始砸时它已把停止原因提示给玩家（抖钱箱 / Inventory full），会话据此停手、本帧不重试
                this.session.ReportCrackOutcome(GeodeMenuContract.TryStartCrack(geodeMenu));

                break;
            case AutoBreakAction.None:
                break;
        }
    }
}
