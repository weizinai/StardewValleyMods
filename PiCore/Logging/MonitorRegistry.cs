using System.Collections.Generic;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Logging;

/// <summary>
/// 唯一的 monitor 注册表：把日志按模组归属路由到各模组自己的 monitor。internal——PiCore 自己的接线，不构成对外 API。
/// </summary>
internal static class MonitorRegistry
{
    private static readonly Dictionary<string, IMonitor> Monitors = new();

    /// <summary>登记模组 monitor，供接收侧按发送方模组 ID 输出对端发来的日志。</summary>
    internal static void Register(string uniqueId, IMonitor monitor)
    {
        Monitors[uniqueId] = monitor;
    }

    internal static IMonitor? Get(string uniqueId)
    {
        return Monitors.GetValueOrDefault(uniqueId);
    }
}
