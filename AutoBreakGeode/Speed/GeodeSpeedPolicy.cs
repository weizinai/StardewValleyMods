using System;
using StardewModdingAPI;
using weizinai.StardewValleyMod.AutoBreakGeode.Config;

namespace weizinai.StardewValleyMod.AutoBreakGeode.Speed;

/// <summary>
/// 砸晶球动画的速度归属：动画速度归谁、倍率上限是多少、本帧实际跑几倍速。
/// </summary>
/// <remarks>
/// 这条事实原先有 4 个副本（配置菜单的上限与适用性、处理器的上限常量与探明「装了没装」、补帧前的再判一次、
/// 配置成员的注释），现在只有这里一处定义、两个消费者各取所需：配置菜单按 <see cref="MaxSpeed" /> 与
/// <see cref="IsExternallyControlled" /> 决定滑条边界与要不要注册这个选项（故菜单不再反向读运行时处理器），
/// 处理器每帧只问 <see cref="SpeedUpTimes" />。
/// </remarks>
internal sealed class GeodeSpeedPolicy
{
    private const string FastAnimationsModId = "Pathoschild.FastAnimations";

    /// <summary>动画加速倍率的上限（1 = 原版速度），与 GMCM 滑条的上限一致。</summary>
    public const int MaxSpeed = 20;

    /// <summary>
    /// 动画速度是否已由别的模组接管（装了快速动画模组即如此）：本模组的补帧整个不生效、倍率恒为 1，
    /// 对应的配置项也因此不注册——不去覆盖玩家在那边调好的速度。
    /// </summary>
    public bool IsExternallyControlled { get; }

    /// <summary>本帧最多跑几倍速（1 = 不补帧，即原版速度）：速度被别的模组接管时恒为 1。</summary>
    /// <remarks>
    /// 取值时自己夹取、不依赖配置项滑条：滑条的 min/max 只约束新输入，该值从 config.json 读出来是原样的
    /// （配置模块不做读取期夹取），老玩家手里那个无边界文本框留下的 100000 会让每帧多调十万次原版 update。
    /// 上限取 <see cref="MaxSpeed" />，与滑条边界同源，省得两处漂移。
    /// </remarks>
    public int SpeedUpTimes => this.IsExternallyControlled ? 1 : Math.Clamp(ModConfig.Instance.BreakGeodeSpeed, 1, MaxSpeed);

    /// <summary>构造策略：探明动画速度有没有被别的模组接管。</summary>
    /// <remarks>
    /// 只在装配期探一次：模组加载列表在游戏启动后就固定了，中途的变化不值得每次取值都去问一遍注册表。
    /// </remarks>
    /// <param name="helper">模组的模组注册表入口。</param>
    public GeodeSpeedPolicy(IModHelper helper)
    {
        this.IsExternallyControlled = helper.ModRegistry.IsLoaded(FastAnimationsModId);
    }
}
