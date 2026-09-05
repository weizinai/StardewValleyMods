using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.PiCore.Patcher;
using xTile.Dimensions;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class PassableMailboxPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<BuildingData>(nameof(BuildingData.IsTilePassable)),
            postfix: this.GetHarmonyMethod(nameof(IsTilePassablePostfix))
        );
        harmony.Patch(
            original: this.RequireMethod<Building>(nameof(Building.doAction)),
            postfix: this.GetHarmonyMethod(nameof(DoActionPostfix))
        );
        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.CanItemBePlacedHere)),
            postfix: this.GetHarmonyMethod(nameof(CanItemBePlacedHerePostfix))
        );
    }

    private static void IsTilePassablePostfix(BuildingData __instance, ref bool __result, int relativeX, int relativeY)
    {
        if (!ModConfig.Instance.PassableMailbox) return;

        if (IsMailboxOnTile(__instance, relativeX, relativeY)) __result = true;
    }

    private static void DoActionPostfix(Building __instance, ref bool __result, Vector2 tileLocation, Farmer who)
    {
        if (!ModConfig.Instance.PassableMailbox) return;

        var data = __instance.GetData();
        if (IsMailboxOnTile(data, (int)(tileLocation.X - __instance.tileX.Value), (int)(tileLocation.Y - __instance.tileY.Value)))
        {
            if (who.currentLocation.performAction("Mailbox", who, new Location((int)tileLocation.X, (int)tileLocation.Y)))
            {
                __result = true;
            }
        }
    }

    private static void CanItemBePlacedHerePostfix(GameLocation __instance, ref bool __result, Vector2 tile)
    {
        if (!ModConfig.Instance.PassableMailbox) return;

        var building = __instance.getBuildingAt(tile);
        if (building is null) return;
        var data = building.GetData();
        if (IsMailboxOnTile(data, (int)(tile.X - building.tileX.Value), (int)(tile.Y - building.tileY.Value)))
        {
            __result = false;
        }
    }

    private static bool IsMailboxOnTile(BuildingData data, int relativeX, int relativeY)
    {
        return data.IndoorMapType == "StardewValley.Locations.Cabin" && data.GetActionAtTile(relativeX, relativeY) == "Mailbox";
    }
}