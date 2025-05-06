/*
 * 代码来源：Pathoschild
 * 原始出处：https://github.com/Pathoschild/StardewMods
 * 授权协议：MIT License
 */

using HarmonyLib;

namespace weizinai.StardewValleyMod.PiCore.Patcher;

/// <summary>A set of Harmony patches to apply.</summary>
public interface IPatcher
{
    /// <summary>Apply the Harmony patches for this instance.</summary>
    /// <param name="harmony">The Harmony instance.</param>
    public void Apply(Harmony harmony);
}