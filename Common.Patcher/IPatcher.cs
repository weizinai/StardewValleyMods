using HarmonyLib;

namespace weizinai.StardewValleyMod.Common.Patcher;

/// <summary>A set of Harmony patches to apply.</summary>
internal interface IPatcher
{
    /// <summary>Apply the Harmony patches for this instance.</summary>
    /// <param name="harmony">The Harmony instance.</param>
    public void Apply(Harmony harmony);
}