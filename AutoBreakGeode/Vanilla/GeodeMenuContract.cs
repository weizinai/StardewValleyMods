using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Vanilla;

/// <summary>
/// 与原版晶球菜单（<see cref="GeodeMenu" />）和游戏循环打交道的交互契约：手里的是不是晶球、怎么点、点了算不算真的开始砸、
/// 怎么把动画跑快、以及游戏暂停或失焦时该不该让行。
/// </summary>
/// <remarks>
/// 本类只回答原版的交互事实，不含本模组的任何决策——「没开始砸所以要停手」「开关状态怎么变」都由调用方决定。
/// 把这些原版怪癖收在一处，是为了让自动砸的主体读起来是策略而不是原版手册：原版升级要重新核对的字段与时序只有这一个文件。
/// 无状态且全静态：事实都从传进来的菜单与 <see cref="Game1" /> 现读，不缓存——窗口尺寸、语言、多人模式都可能中途改变。
/// </remarks>
internal static class GeodeMenuContract
{
    /// <summary>原版此刻认定手里拿着的是能砸的晶球（含金色椰子、谜之盒等同类物）。</summary>
    /// <param name="geodeMenu">当前活动菜单。</param>
    /// <returns>手里拿的是不是晶球。</returns>
    public static bool IsHoldingGeode(GeodeMenu geodeMenu)
    {
        return Utility.IsGeode(geodeMenu.heldItem);
    }

    /// <summary>原版此刻是否正在放砸晶球的动画（动画计时器尚未走完）。</summary>
    /// <param name="geodeMenu">当前活动菜单。</param>
    /// <returns>动画是否还在跑。</returns>
    public static bool IsCracking(GeodeMenu geodeMenu)
    {
        return geodeMenu.geodeAnimationTimer > 0;
    }

    /// <summary>
    /// 点一次晶球区，并回答这一击是否真的让原版开始砸。
    /// </summary>
    /// <remarks>
    /// 是否真的开始砸，交给原版判定而不是自己复刻条件：钱不够与背包满都由原版拒绝并给出它自己的提示
    /// （<c>GeodeMenu.cs:159</c> 装不下时挂出 Inventory full，<c>:167</c> 钱不够时抖钱箱 1s），
    /// 复刻那份判据只会在原版改动时悄悄漂移。金椰那一帧原版只置 <c>waitingForServerResponse</c>（<c>:140</c>），
    /// 真正的 <c>startGeodeCrack</c> 要等互斥锁回调里才置回该标志并起动画（<c>:143-148</c>），
    /// 故成功判据里必须有它，否则会把等锁误判成失败而白停一次。
    /// </remarks>
    /// <param name="geodeMenu">当前活动菜单。</param>
    /// <returns>true = 原版真的开始砸了（已起动画或正在等互斥锁回调）；false = 这一击被原版拒绝。</returns>
    public static bool TryStartCrack(GeodeMenu geodeMenu)
    {
        var timerBeforeClick = geodeMenu.geodeAnimationTimer;
        var x = geodeMenu.geodeSpot.bounds.Center.X;
        var y = geodeMenu.geodeSpot.bounds.Center.Y;

        geodeMenu.receiveLeftClick(x, y);

        return geodeMenu.waitingForServerResponse || geodeMenu.geodeAnimationTimer != timerBeforeClick;
    }

    /// <summary>
    /// 同一 tick 内多调几次原版 <c>update</c>，把砸晶球的动画跑快（<paramref name="speed" /> = 1 即原版速度）。
    /// </summary>
    /// <remarks>
    /// 手法参照 <c>FastAnimations</c> 的 <c>BreakGeodeHandler</c>：跑快靠的是多调几次 <c>update</c>，而不是缩短动画计时器
    /// 的递减量，故每一帧的逻辑（帧音效、开壳那一帧的转换与计数）都会被跑到、不跳过；代价是同一 tick 内推进多帧时，
    /// 只有最后一帧会被画出来。一次 <c>update</c> 至多推进一帧（<c>AnimatedSprite.animateOnce</c> 里只有一个
    /// <c>timer &lt;= 0</c> 分支，<c>AnimatedSprite.cs:503</c>），故倍率的语义是「每帧最多跑几倍速」。
    /// 调用方负责夹取倍率并决定要不要加速（装了快速动画模组时全交给它）。
    /// </remarks>
    /// <param name="geodeMenu">当前活动菜单。</param>
    /// <param name="speed">每帧最多跑几倍速，1 = 原版速度。</param>
    public static void AdvanceCrackingAnimation(GeodeMenu geodeMenu, int speed)
    {
        for (var i = 1; i < speed; i++)
        {
            geodeMenu.update(Game1.currentGameTime);
        }
    }

    /// <summary>
    /// 游戏是否处于「原版自己都不推进」的状态（暂停，或设置里的失焦暂停），此时本模组也该让行。
    /// </summary>
    /// <remarks>
    /// 判据取自原版 <c>Game1._update</c>（由 <c>Game1.Update</c> 调用）的失焦暂停，与之等价：反编译源码 <c>Game1.cs:3896</c>。原版写成两个合取项
    /// <c>(paused || (!IsActiveNoOverlay &amp;&amp; releaseBuild))</c> 与
    /// <c>(options == null || pauseWhenOutOfFocus || paused)</c>，本方法把两者折叠成
    /// <c>paused || (!IsActiveNoOverlay &amp;&amp; pauseWhenOutOfFocus)</c>：令 P = <c>paused</c>、A = <c>IsActiveNoOverlay</c>、
    /// O = <c>pauseWhenOutOfFocus</c>，则 <c>(P || ¬A) &amp;&amp; (P || O) ≡ P || (¬A &amp;&amp; O)</c>，折叠不改变判定。
    /// 折叠丢掉的两处也不影响判定：模组跑的也是发布版，<c>releaseBuild</c> 恒真故不必再判；<c>options == null</c> 那半个守卫
    /// 是原版给极早期 tick 的防御，本方法只在晶球菜单已开时被调用，<c>Game1.options</c> 必非空。<c>IsActiveNoOverlay</c> 是
    /// <see cref="Game1" /> 的实例属性（原版自己也写 <c>this.IsActiveNoOverlay</c>），故经 <c>Game1.game1</c> 取。
    /// 需要它有两个理由。其一，原版那道 return 只拦 <c>Game1._update</c>：SMAPI 的 <c>UpdateTicked</c> 照旧抛、本模组的自动砸照旧跑，
    /// 而输入事件只在窗口有焦点时抛——失焦挂机时玩家按快捷键刹不住车。其二，暂停帧里 <c>Game1.currentGameTime</c> 还是上一帧的旧值
    /// （原版在 return 之后才赋值，<c>Game1.cs:3905</c>），补帧若照跑就是拿旧时间戳继续推进动画。调用方据此只跳过自动砸、
    /// **保留开关状态**，切回游戏即接着砸，与游戏的暂停语义一致（多人模式原版本就不暂停，故照跑）。
    /// </remarks>
    /// <returns>是否应当让行、跳过本帧的自动砸。</returns>
    public static bool ShouldYieldToGamePause()
    {
        return (Game1.paused || !Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) && Game1.multiplayerMode == 0;
    }
}
