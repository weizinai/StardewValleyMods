using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Mining;

internal class CoolLavaHandler : BaseAutomationHandler
{
    public CoolLavaHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        if (location is not VolcanoDungeon dungeon) return;
        var wateringCan = ToolHelper.GetTool<WateringCan>(this.Config.AutoCoolLava.FindToolFromInventory);
        if (wateringCan is null) return;

        var hasAddWaterMessage = true;
        var grid = this.GetTileGrid(this.Config.AutoCoolLava.Range);
        foreach (var tile in grid)
        {
            if (wateringCan.WaterLeft <= 0)
            {
                if (!hasAddWaterMessage) Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                break;
            }

            hasAddWaterMessage = false;

            if (player.Stamina <= this.Config.AutoCoolLava.StopStamina) return;
            if (!this.CanCoolLave(dungeon, tile)) continue;
            this.UseToolOnTile(location, player, wateringCan, tile);
            if (wateringCan.WaterLeft > 0 && player.ShouldHandleAnimationSound())
                player.playNearbySoundLocal("wateringCan");
        }
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