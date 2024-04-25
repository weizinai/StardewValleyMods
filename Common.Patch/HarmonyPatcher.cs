using HarmonyLib;
using StardewModdingAPI;

namespace Common.Patch;

public class HarmonyPatcher
{
    public static void Patch(Mod mod, params IPatcher[] patchers)
    {
        var harmony = new Harmony(mod.ModManifest.UniqueID);

        foreach (var patcher in patchers)
        {
            try
            {
                patcher.Patch(harmony);
            }
            catch (Exception ex)
            {
                mod.Monitor.Log($"Failed to apply '{patcher.GetType().FullName}' patcher; some features may not work correctly. Technical details:\n{ex}", LogLevel.Error);
            }
        }
    }
}