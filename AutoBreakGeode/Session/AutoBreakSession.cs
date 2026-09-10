using weizinai.StardewValleyMod.AutoBreakGeode.Vanilla;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Session;

/// <summary>
/// 自动砸的会话：持有开关标志，并按本帧捕获的晶球菜单现场给出「本帧该做什么」。
/// </summary>
/// <remarks>
/// 开关标志原先有 5 个写点散在 4 个方法里（菜单不在、切换开关的两个分支、晶球被收回、原版拒绝），
/// 现在只有本类的转移方法写它，停止规则也只剩这里一处定义；处理器的职责退化回「把事件转进来、
/// 把结论翻译成对原版菜单的动作」。
/// 本类自己不读游戏状态：输入是 <see cref="GeodeContext" /> 这份现场快照与「要不要让行」，对原版的唯一接触是向契约问一句
/// 「动画还在跑吗」，故它描述的是决策而不是原版手册。
/// 转移次序不变量见 <see cref="Update" /> 的 remarks。
/// </remarks>
internal sealed class AutoBreakSession
{
    private bool isAutoBreaking;

    /// <summary>当前是否正在自动砸：按钮文案与叠层的可见性据此取值。</summary>
    public bool IsAutoBreaking => this.isAutoBreaking;

    /// <summary>
    /// 切换开关——按钮与快捷键共用这一个入口（同源），故两边的文字与状态永远一致：
    /// 开→关随时放行；关→开需现场手里有晶球，没手持即 no-op（不翻转标志，按钮文字也就不会闪烁）。
    /// </summary>
    /// <remarks>这里只判「有没有晶球」，能不能砸（钱够不够、装不装得下）留给原版在点击时判定并给出它自己的提示。</remarks>
    /// <param name="context">捕获于本次输入事件的菜单现场。</param>
    public void Toggle(GeodeContext context)
    {
        if (this.isAutoBreaking)
        {
            this.isAutoBreaking = false;

            return;
        }

        if (context.IsHoldingGeode)
        {
            this.isAutoBreaking = true;
        }
    }

    /// <summary>按本帧的现场推进状态，并给出本帧要做的唯一一件事。</summary>
    /// <remarks>
    /// 转移次序有意固定，与重构前的处理器逐条对应：现场丢失（菜单不在）**先**判、且无条件停止；让行**后**判、
    /// 且只跳过本帧。让行不动开关是有意的——暂停或失焦只是游戏这一帧不推进，切回来就该接着砸
    /// （多人模式原版本就不暂停，故照跑）。「手里没有晶球」同属现场丢失：玩家把晶球收回背包或换成别的物品时
    /// 静默停手，不再每帧空点一次原版菜单。
    /// </remarks>
    /// <param name="context">本帧捕获的菜单现场。</param>
    /// <param name="shouldYieldToPause">本帧是否应让行（暂停，或设置里的失焦暂停），判据见 <see cref="GeodeMenuContract.ShouldYieldToGamePause" />；调用方只在开关开着且晶球菜单在时求值（那两处守卫先判，用不上就不必问）。</param>
    /// <returns>本帧要做的唯一一件事。</returns>
    public AutoBreakAction Update(GeodeContext context, bool shouldYieldToPause)
    {
        var menu = context.Menu;

        // 菜单不在（或被其它菜单取代）即停：既不残留绘制与命中，也不在菜单外继续自动砸
        if (menu is null)
        {
            this.isAutoBreaking = false;

            return AutoBreakAction.None;
        }

        if (shouldYieldToPause || !this.isAutoBreaking) return AutoBreakAction.None;

        if (!context.IsHoldingGeode)
        {
            this.isAutoBreaking = false;

            return AutoBreakAction.None;
        }

        return GeodeMenuContract.IsCracking(menu) ? AutoBreakAction.SpeedUp : AutoBreakAction.Crack;
    }

    /// <summary>回报本帧那一击的结果：原版没真的开始砸（钱不够 / 背包满）即停手。</summary>
    /// <remarks>
    /// 是否真的开始砸由原版判定而不是本模组复刻条件，故这里只消费结论。停手而不是重试：原版在这一击里已经把停止原因
    /// 提示给玩家（抖钱箱 1s / Inventory full），每帧重试只会把那份提示刷成噪音。
    /// </remarks>
    /// <param name="started">这一击有没有真的让原版开始砸。</param>
    public void ReportCrackOutcome(bool started)
    {
        if (!started) this.isAutoBreaking = false;
    }
}
