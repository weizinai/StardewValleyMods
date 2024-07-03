using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Patcher;

namespace CustomCabinFix.Patcher;

internal class BuildingPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<Building>(nameof(Building.getPorchStandingSpot)),
            postfix: this.GetHarmonyMethod(nameof(GetPorchStandingSpotPostfix))
        );
    }

    private static void GetPorchStandingSpotPostfix(Building __instance, ref Point __result)
    {
        if (__instance.GetIndoors() is Cabin)
        {
            __result = new Point(__instance.tileX.Value + __instance.humanDoor.Value.X, 
                __instance.tileY.Value + __instance.humanDoor.Value.Y + 1);
        }
    }
}