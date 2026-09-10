using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.AutoBreakGeode.Config;
using weizinai.StardewValleyMod.AutoBreakGeode.UI;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Handler;

/// <summary>
/// 晶球相关逻辑的唯一持有者：自动砸晶球的状态机（开关标志、触发、停止契约、动画加速）与承载开关按钮的
/// <see cref="AutoBreakButtonOverlay" />，并自行订阅驱动它的两个事件。模组入口只负责装配。
/// </summary>
/// <remarks>
/// 开关标志是本处理器的运行期状态，故它**不进**配置变更重建流程（模组入口只 <c>Apply</c> 一次、不重建）：
/// 本模组没有任何按配置构建的东西，速度选项在循环里现读、快捷键在事件里现查，重建只会白丢一次开关状态。
/// </remarks>
internal class AutoBreakHandler : BaseHandler
{
    private const string FastAnimationsModId = "Pathoschild.FastAnimations";

    /// <summary>动画加速倍率的上限（1 = 原版速度），与 GMCM 滑条的上限一致。</summary>
    internal const int MaxGeodeSpeed = 20;

    /// <summary>
    /// 快速动画模组是否已加载：装了就把动画速度全交给它，本模组不补帧——不去覆盖玩家在它里面调好的速度，
    /// 对应的配置项也因此不注册。
    /// </summary>
    internal bool IsFastAnimationsLoaded { get; }

    private readonly AutoBreakButtonOverlay beginButtonOverlay;

    private bool isAutoBreaking;

    /// <summary>构造处理器：查快速动画模组是否已加载，并建好开始按钮叠层。</summary>
    /// <param name="helper">模组的事件与输入入口。</param>
    public AutoBreakHandler(IModHelper helper) : base(helper)
    {
        this.IsFastAnimationsLoaded = helper.ModRegistry.IsLoaded(FastAnimationsModId);
        this.beginButtonOverlay = new AutoBreakButtonOverlay(helper.Events.Display, this.ToggleAutoBreaking);
    }

    /// <inheritdoc />
    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    /// <inheritdoc />
    public override void Clear()
    {
        this.helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;
        this.helper.Events.Input.ButtonPressed -= this.OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (Game1.activeClickableMenu is not GeodeMenu) return;

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
        var geodeMenu = Game1.activeClickableMenu as GeodeMenu;

        // 叠层只在晶球菜单里出现：可见性直接跟随活动菜单身份，下面同一条判据即可驱动锚位
        this.beginButtonOverlay.IsShown = geodeMenu is not null;

        if (geodeMenu is null)
        {
            // 菜单不在（或被其它菜单取代）即停：既不残留绘制与命中，也不在菜单外继续自动砸
            this.isAutoBreaking = false;
        }
        else
        {
            this.UpdateAutoBreak(geodeMenu);
        }

        // 文字由标志直接推导并每帧同步，且置于自动砸逻辑之后：自动停止（无晶球 / 钱不够 / 背包满）与快捷键切换都在同一帧翻正，不闪烁
        this.beginButtonOverlay.SyncText(this.isAutoBreaking);

        if (geodeMenu is not null)
        {
            this.UpdateOverlay(geodeMenu);
        }
    }

    /// <summary>
    /// 每帧把叠层贴到面板左缘外侧、与晶球区顶边齐平并路由悬停。顺序有意固定为「先悬停（冲刷挂载布局）、后取锚位」：
    /// 文字刚在上一句切换（开始/停止）时，锚位按新文案的实测宽度右对齐，不会有一帧压进面板。
    /// 悬停不按窗口焦点分流：失焦时 SMAPI 不再刷新输入状态，读到的仍是失焦前的光标，路由结果自然停在原处，
    /// 与原版「失焦不派发悬停」的可见效果一致；而这一步同时承担冲刷布局的职责，跳过会让锚位停在旧宽度上。
    /// </summary>
    /// <param name="geodeMenu">当前活动菜单。</param>
    private void UpdateOverlay(GeodeMenu geodeMenu)
    {
        this.beginButtonOverlay.PerformHoverAction(Game1.getMouseX(), Game1.getMouseY());
        this.beginButtonOverlay.AnchorOutsidePanel(geodeMenu.xPositionOnScreen, geodeMenu.geodeSpot.bounds.Y);
    }

    /// <summary>
    /// 切换自动砸开关——按钮与快捷键共用这一个入口（同源），故两边的文字与状态永远一致：
    /// 开→关随时放行；关→开需当前手持晶球，未手持即 no-op（不翻转标志，按钮文字也就不会闪烁）。
    /// 这里只判「有没有晶球」，能不能砸（钱够不够、装不装得下）留给原版在点击时判定并给出它自己的提示。
    /// </summary>
    private void ToggleAutoBreaking()
    {
        if (this.isAutoBreaking)
        {
            this.isAutoBreaking = false;

            return;
        }

        if (Game1.activeClickableMenu is GeodeMenu geodeMenu && Utility.IsGeode(geodeMenu.heldItem))
        {
            this.isAutoBreaking = true;
        }
    }

    /// <summary>
    /// 自动砸循环：手持晶球时在动画结束后点击晶球区再砸一颗，未装快速动画模组则补帧加速；
    /// 无晶球即停，点击后若原版并未真的开始砸（钱不够 / 背包满 / 等锁之外的拒绝）也停。
    /// 游戏暂停或窗口失焦时整体让行，详见 <see cref="ShouldPauseForFocus" />。
    /// </summary>
    /// <param name="geodeMenu">当前活动菜单。</param>
    private void UpdateAutoBreak(GeodeMenu geodeMenu)
    {
        if (!this.isAutoBreaking || this.ShouldPauseForFocus()) return;

        // 玩家把晶球收回背包（或换成别的物品）时静默停止：不再每帧空点一次原版菜单
        if (!Utility.IsGeode(geodeMenu.heldItem))
        {
            this.isAutoBreaking = false;

            return;
        }

        if (geodeMenu.geodeAnimationTimer > 0)
        {
            this.SpeedUpAnimation(geodeMenu);

            return;
        }

        // 是否真的开始砸了，交给原版判定而不是自己复刻条件：钱不够与背包满都由原版拒绝并给出自己的提示
        // （GeodeMenu.receiveLeftClick：钱不够抖钱箱 1s、装不下显示 Inventory full 并抖 1.5s），
        // 复刻那份判据只会在原版改动时悄悄漂移。金椰那一帧原版只置 waitingForServerResponse，
        // 真正的 startGeodeCrack 要等互斥锁回调，故成功判据里必须有它，否则会把等锁误判成失败而白停一次。
        var timerBeforeClick = geodeMenu.geodeAnimationTimer;
        var x = geodeMenu.geodeSpot.bounds.Center.X;
        var y = geodeMenu.geodeSpot.bounds.Center.Y;
        geodeMenu.receiveLeftClick(x, y);

        if (!geodeMenu.waitingForServerResponse && geodeMenu.geodeAnimationTimer == timerBeforeClick)
        {
            // 原版没开始砸：本帧它已把停止原因提示给玩家（抖钱箱 / Inventory full），这里直接停，不再重试
            this.isAutoBreaking = false;
        }
    }

    /// <summary>
    /// 参照 <c>FastAnimations</c> 的 <c>BreakGeodeHandler</c>：同一 tick 内多调几次原版 <c>update</c> 把动画跑快
    /// （不是缩短 <c>geodeAnimationTimer</c> 的递减量，故动画帧一个不落）。一次 <c>update</c> 至多推进一帧，
    /// 所以 <see cref="ModConfig.BreakGeodeSpeed" /> 的语义是「每帧最多跑几倍速」——它只在没装 Fast Animations 时生效，
    /// 装了就以那边为准，见 <see cref="IsFastAnimationsLoaded" />。
    /// </summary>
    /// <param name="geodeMenu">当前活动菜单。</param>
    private void SpeedUpAnimation(GeodeMenu geodeMenu)
    {
        if (this.IsFastAnimationsLoaded) return;

        // 循环边界自己夹取，不依赖配置项滑条：滑条的 min/max 只约束新输入，该值从 config.json 读出来是原样的
        // （配置模块不做读取期夹取），老玩家手里那个无边界文本框留下的 100000 会让每帧多调十万次原版 update
        var speed = Math.Clamp(ModConfig.Instance.BreakGeodeSpeed, 1, MaxGeodeSpeed);

        for (var i = 1; i < speed; i++)
        {
            geodeMenu.update(Game1.currentGameTime);
        }
    }

    /// <summary>
    /// 判据取自原版 <c>Game1.Update</c> 的失焦暂停（反编译源码 <c>Game1.cs:3896</c>），与之等价。原版写成两个合取项
    /// <c>(paused || (!IsActiveNoOverlay &amp;&amp; releaseBuild))</c> 与
    /// <c>(options == null || pauseWhenOutOfFocus || paused)</c>，本方法把两者折叠成
    /// <c>paused || (!IsActiveNoOverlay &amp;&amp; pauseWhenOutOfFocus)</c>：令 P = <c>paused</c>、A = <c>IsActiveNoOverlay</c>、
    /// O = <c>pauseWhenOutOfFocus</c>，则 <c>(P || ¬A) &amp;&amp; (P || O) ≡ P || (¬A &amp;&amp; O)</c>，折叠不改变判定。
    /// 折叠丢掉的两处也不影响判定：模组跑的也是发布版，<c>releaseBuild</c> 恒真故不必再判；<c>options == null</c> 那半个守卫
    /// 是原版给极早期 tick 的防御，本方法只在晶球菜单已开时被调用，<c>Game1.options</c> 必非空。<c>IsActiveNoOverlay</c> 是
    /// <see cref="Game1" /> 的实例属性（原版自己也写 <c>this.IsActiveNoOverlay</c>），故经 <c>Game1.game1</c> 取。
    /// 需要它是因为原版那道 return 只拦 <c>Game1.Update</c>：SMAPI 的 <c>UpdateTicked</c> 照旧抛、本模组的自动砸照旧跑，
    /// 而输入事件只在窗口有焦点时抛——失焦挂机时玩家按快捷键刹不住车。这里只跳过自动砸、**保留开关状态**，
    /// 切回游戏即接着砸，与游戏的暂停语义一致（多人模式原版本就不暂停，故照跑）。
    /// </summary>
    /// <returns>是否应当跳过本帧的自动砸。</returns>
    private bool ShouldPauseForFocus()
    {
        return (Game1.paused || !Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) && Game1.multiplayerMode == 0;
    }
}
