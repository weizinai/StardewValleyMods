using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class CoolLavaHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (location is not VolcanoDungeon dungeon) return;

        var wateringCan = ToolHelper.GetTool<WateringCan>(this.Config.AutoCoolLava.FindToolFromInventory);
        if (wateringCan is null) return;

        this.ForEachTile(this.Config.AutoCoolLava.Range, tile =>
        {
            if (wateringCan.WaterLeft <= 0) return false;
            if (player.Stamina <= this.Config.AutoCoolLava.StopStamina) return false;

            if (this.CanCoolLave(dungeon, tile))
            {
                this.UseToolOnTile(location, player, wateringCan, tile);
                if (player.ShouldHandleAnimationSound()) player.playNearbySoundLocal("wateringCan");
            }

            return true;
        });
    }

    private bool CanCoolLave(VolcanoDungeon dungeon, Vector2 tile)
    {
        var x = (int)tile.X;
        var y = (int)tile.Y;
        return !dungeon.CanRefillWateringCanOnTile(x, y) &&
               dungeon.isTileOnMap(tile) &&
               dungeon.waterTiles[x, y] &&
               !dungeon.cooledLavaTiles.ContainsKey(tile);
    }
}