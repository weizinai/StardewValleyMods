using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.CustomCabinFix.Patcher;

internal class BuildingPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Building>(nameof(Building.getPorchStandingSpot)),
            postfix: this.GetHarmonyMethod(nameof(GetPorchStandingSpotPostfix))
        );
    }

    private static void GetPorchStandingSpotPostfix(Building __instance, ref Point __result)
    {
        if (__instance.IsCabin(out _))
        {
            __result = new Point(
                __instance.tileX.Value + __instance.humanDoor.Value.X,
                __instance.tileY.Value + __instance.humanDoor.Value.Y + 1
            );
        }
    }
}