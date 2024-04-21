using HarmonyLib;

namespace Common.Patch;

public static class HarmonyPatcher
{
    public static Harmony Patch()
    {
        var harmony = new Harmony("com.github.aleksejmanilo.common");
        harmony.PatchAll();
        return harmony;
    }
}