/*
 * 代码来源：Pathoschild
 * 原始出处：https://github.com/Pathoschild/StardewMods
 * 授权协议：MIT License
 */

using System;
using HarmonyLib;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>Simplifies applying <see cref="IPatcher"/> instances to the game.</summary>
public static class HarmonyPatcher
{
    /// <summary>Apply the given Harmony patchers.</summary>
    /// <param name="uniqueId">The unique id of mod applying the patchers.</param>
    /// <param name="patchers">The patchers to apply.</param>
    public static void Apply(string uniqueId, params IPatcher[] patchers)
    {
        var harmony = new Harmony(uniqueId);

        foreach (var patcher in patchers)
        {
            try
            {
                patcher.Apply(harmony);
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to apply '{patcher.GetType().FullName}' patcher. Technical details:\n{e}");
            }
        }
    }
}