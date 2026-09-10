using System;

namespace weizinai.StardewValleyMod.CustomMineRefresh.Policy;

/// <summary>
/// 刷新节拍：把配置里的「游戏分钟」归一化成实际生效的节拍，并按累计的游戏分钟回答本次事件要不要扫一遍。
/// </summary>
/// <remarks>
/// 纯逻辑，不引用任何游戏类型：时钟跳的粒度由调用方按事件驱动，本类只认「又过了 10 游戏分钟」这件事，
/// 跨天归零因此也只是一个显式动作。矿井与火山各持一个实例，互不影响；累计只在被调用时推进，
/// 调用方关掉某套机制期间不来问，那段时间就不计入该机制的节拍。
/// </remarks>
internal sealed class RefreshSchedule
{
    /// <summary>游戏时钟跳一次的步长（游戏分钟）：原版时钟每次跳 10 游戏分钟，现实里约 7 秒。</summary>
    public const int ClockTickMinutes = 10;

    /// <summary>节拍上限（10 游戏小时）：再大的值也只等于「一天最多扫一次」，没有意义。</summary>
    public const int MaxIntervalMinutes = 600;

    /// <summary>按当前节拍累计的游戏分钟。</summary>
    private int elapsedMinutes;

    /// <summary>把配置值归一化成实际生效的节拍。</summary>
    /// <param name="configuredMinutes">配置里的原始值（可能越界，也可能不是 10 的倍数）。</param>
    /// <returns>0（即时）或 10–600 之间 10 的倍数。</returns>
    public static int Normalize(int configuredMinutes)
    {
        var clamped = Math.Clamp(configuredMinutes, 0, MaxIntervalMinutes);

        // 时钟只在整 10 分钟上跳，节拍因而只可能以 10 分钟为步长：15 这样的值向上取整到 20，而不是静默失效
        return (clamped + ClockTickMinutes - 1) / ClockTickMinutes * ClockTickMinutes;
    }

    /// <summary>该节拍是否表示「即时」（每秒检查一次，而不是按游戏分钟定时）。</summary>
    /// <param name="configuredMinutes">配置里的原始值。</param>
    public static bool IsImmediate(int configuredMinutes)
    {
        return Normalize(configuredMinutes) == 0;
    }

    /// <summary>按当前节拍回答本次事件是否到点。</summary>
    /// <param name="configuredMinutes">配置里的原始值（每次现读，玩家改完配置即时生效）。</param>
    /// <param name="isClockTick"><c>true</c> 表示本次是游戏时钟跳，<c>false</c> 表示每秒的检查。</param>
    /// <returns>本次是否该扫一遍。</returns>
    public bool IsDue(int configuredMinutes, bool isClockTick)
    {
        var intervalMinutes = Normalize(configuredMinutes);

        // 两档节拍各由一件事驱动：即时只认每秒的检查（时钟跳不对它负责），按分钟只认时钟跳
        if (intervalMinutes == 0) return !isClockTick;
        if (!isClockTick) return false;

        this.elapsedMinutes += ClockTickMinutes;

        if (this.elapsedMinutes < intervalMinutes) return false;

        // 减掉一档而不是清零：不满一档的零头继续参与下一次累计
        this.elapsedMinutes -= intervalMinutes;

        return true;
    }

    /// <summary>把累计值归零（跨天时由调用方触发，免得跨天漏刷或多刷）。</summary>
    public void Reset()
    {
        this.elapsedMinutes = 0;
    }
}
