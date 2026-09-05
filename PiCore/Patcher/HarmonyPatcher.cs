using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>
/// 简化将 <see cref="IPatcher" /> 实例应用到游戏的过程。
/// 每个补丁独立应用并隔离异常：失败不影响其余补丁，错误按调用模组的 uniqueId 路由到其 monitor。
/// </summary>
public static class HarmonyPatcher
{
    /// <summary>
    /// 应用给定的 Harmony 补丁。
    /// </summary>
    /// <param name="mod">应用补丁的模组。</param>
    /// <param name="patchers">要应用的补丁。</param>
    public static void Apply(Mod mod, params IPatcher[] patchers)
    {
        var uniqueId = mod.ModManifest.UniqueID;
        var harmony = new Harmony(uniqueId);
        var monitor = mod.Monitor;

        var applied = new List<string>();
        var failed = new List<string>();

        foreach (var patcher in patchers)
        {
            try
            {
                if (!patcher.IsEnabled)
                {
                    monitor?.Log($"Skipped disabled patcher '{patcher.Name}'.");

                    continue;
                }

                patcher.Apply(harmony);
                applied.Add(patcher.Name);
                monitor?.Log($"Applied patch '{patcher.Name}'.");
            }
            catch (Exception e)
            {
                failed.Add(patcher.Name);
                monitor?.Log($"Failed to apply '{patcher.Name}' patcher. Technical details:\n{e}", LogLevel.Error);
            }
        }

        if (failed.Count > 0)
        {
            monitor?.Log($"Applied {applied.Count} of {patchers.Length} patcher(s), {failed.Count} failed: [{string.Join(", ", failed)}].", LogLevel.Error);
        }
        else if (applied.Count > 0)
        {
            monitor?.Log($"Applied {applied.Count} patcher(s): [{string.Join(", ", applied)}].");
        }
    }
}
