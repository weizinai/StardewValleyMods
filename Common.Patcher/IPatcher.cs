using HarmonyLib;

namespace Common.Patcher;

internal interface IPatcher
{
    /// <summary>Apply the Harmony patches for this instance.</summary>
    /// <param name="harmony">The Harmony instance.</param>
    public void Apply(Harmony harmony);
}