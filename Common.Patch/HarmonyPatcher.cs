using HarmonyLib;
using StardewModdingAPI;

namespace Common.Patch;

internal static class HarmonyPatcher
{
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