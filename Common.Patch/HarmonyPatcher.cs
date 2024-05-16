using HarmonyLib;
using StardewModdingAPI;

namespace Common.Patch;

/// <summary>Simplifies applying <see cref="IPatcher"/> instances to the game.</summary>
internal static class HarmonyPatcher
{
    /// <summary>Apply the given Harmony patchers.</summary>
    /// <param name="mod">The mod applying the patchers.</param>
    /// <param name="patchers">The patchers to apply.</param>
    public static void Apply(Mod mod, params IPatcher[] patchers)
    {
        var harmony = new Harmony(mod.ModManifest.UniqueID);

        foreach (var patcher in patchers)
        {
            try
            {
                patcher.Apply(harmony);
            }
            catch (Exception ex)
            {
                mod.Monitor.Log($"Failed to apply '{patcher.GetType().FullName}' patcher. Technical details:\n{ex}", LogLevel.Error);
            }
        }
    }
}