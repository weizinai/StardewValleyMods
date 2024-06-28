using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class CarpenterMenuPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public CarpenterMenuPatcher(ModConfig config)
    {
        CarpenterMenuPatcher.config = config;
    }
    
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<CarpenterMenu>(nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
            this.GetHarmonyMethod(nameof(ReturnToCarpentryMenuAfterSuccessfulBuildPrefix))
        );
    }

    private static bool ReturnToCarpentryMenuAfterSuccessfulBuildPrefix(CarpenterMenu __instance)
    {
        if (!config.BuildCabinContinually) return true;
        
        if (__instance.currentBuilding.GetIndoors() is Cabin && __instance.CanBuildCurrentBlueprint())
        {
            __instance.freeze = false;
            return false;
        }
        
        return true;
    }
}