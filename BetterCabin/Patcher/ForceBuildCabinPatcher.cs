using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class ForceBuildCabinPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.isBuildable)),
            postfix: this.GetHarmonyMethod(nameof(IsBuildablePostfix))
        );

        harmony.Patch(
            original: this.RequireMethod<GameLocation>(
                nameof(GameLocation.buildStructure),
                new[] { typeof(string), typeof(BuildingData), typeof(Vector2), typeof(Farmer), typeof(Building).MakeByRefType(), typeof(bool), typeof(bool) }
            ),
            postfix: this.GetHarmonyMethod(nameof(BuildStructurePostfix))
        );
    }

    private static void IsBuildablePostfix(GameLocation __instance, ref bool __result, Vector2 tileLocation)
    {
        if (!ModConfig.Instance.ForceBuildCabin) return;

        if (__result) return;

        if (Game1.activeClickableMenu is CarpenterMenu { Action: CarpenterMenu.CarpentryAction.None } menu)
        {
            if (menu.Blueprint.Data.IndoorMapType == "StardewValley.Locations.Cabin")
            {
                if (__instance.getObjectAtTile((int)tileLocation.X, (int)tileLocation.Y)?.Category == SObject.litterCategory)
                {
                    __result = true;
                }
            }
        }
    }

    private static void BuildStructurePostfix(GameLocation __instance, ref bool __result, BuildingData data, Vector2 tileLocation)
    {
        if (!ModConfig.Instance.ForceBuildCabin) return;

        if (__result == false) return;

        if (data.IndoorMapType != "StardewValley.Locations.Cabin") return;

        for (var x = 0; x < data.Size.X; x++)
        {
            for (var y = 0; y < data.Size.Y; y++)
            {
                var position = new Vector2(tileLocation.X + x, tileLocation.Y + y);
                if (__instance.objects.GetValueOrDefault(position)?.Category == SObject.litterCategory)
                {
                    __instance.objects.Remove(position);
                }
            }
        }
    }
}