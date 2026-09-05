using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
/// 日志工具：HUD 消息与各模组 monitor 共享注册表。
/// 控制台日志经泛型 <see cref="Logger{T}"/> 发出，保证每个模组一份独立状态。
/// </summary>
public static class HudLogger
{
    /// <summary>
    /// 模组 uniqueId 到 monitor 的注册表，供补丁失败报错与多人消息按模组归属路由。
    /// </summary>
    private static readonly Dictionary<string, IMonitor> monitorRegistry = new();

    /// <summary>
    /// 全局兜底 monitor：PiCore 总是最先加载，首个注册者即为兜底。
    /// </summary>
    private static IMonitor? fallbackMonitor;

    /// <summary>
    /// 获取全局兜底 monitor。
    /// </summary>
    internal static IMonitor? FallbackMonitor => fallbackMonitor;

    /// <summary>
    /// 显示无图标 HUD 提示。
    /// </summary>
    /// <param name="message">提示内容。</param>
    /// <param name="timeLeft">显示时长（毫秒）。</param>
    public static void NoIconHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, timeLeft) { noIcon = true });
    }

    /// <summary>
    /// 显示错误 HUD 提示。
    /// </summary>
    /// <param name="message">提示内容。</param>
    /// <param name="timeLeft">显示时长（毫秒）。</param>
    public static void ErrorHUDMessage(string message, float timeLeft = 3500f)
    {
        Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = timeLeft });
    }

    /// <summary>
    /// 登记模组 monitor 到共享注册表；首个注册者作为全局兜底。
    /// </summary>
    /// <param name="uniqueId">模组唯一ID。</param>
    /// <param name="monitor">模组的 monitor。</param>
    internal static void Register(string uniqueId, IMonitor monitor)
    {
        monitorRegistry[uniqueId] = monitor;
        fallbackMonitor ??= monitor;
    }

    /// <summary>
    /// 按模组唯一ID查询其 monitor。
    /// </summary>
    /// <param name="uniqueId">模组唯一ID。</param>
    /// <returns>对应模组的 monitor；未注册时返回 null。</returns>
    internal static IMonitor? GetMonitor(string uniqueId)
    {
        return monitorRegistry.GetValueOrDefault(uniqueId);
    }
}