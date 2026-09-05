using HarmonyLib;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class CarpenterMenuPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<CarpenterMenu>(nameof(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild)),
            prefix: this.GetHarmonyMethod(nameof(ReturnToCarpentryMenuAfterSuccessfulBuildPrefix))
        );
    }

    private static bool ReturnToCarpentryMenuAfterSuccessfulBuildPrefix(CarpenterMenu __instance)
    {
        if (!ModConfig.Instance.BuildCabinContinually) return true;

        // __instance.isCabin 无法判断自定义小屋
        // __instance.GetIndoors() is Cabin 此时室内还未生成，无法判断是否为小屋
        if (__instance.Blueprint.Data.IndoorMapType == "StardewValley.Locations.Cabin" && __instance.CanBuildCurrentBlueprint())
        {
            __instance.freeze = false;
            return false;
        }

        return true;
    }
}