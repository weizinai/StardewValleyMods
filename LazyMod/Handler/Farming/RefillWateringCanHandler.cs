using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Farming;

internal class RefillWateringCanHandler : BaseAutomationHandler
{
    public RefillWateringCanHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var wateringCan = ToolHelper.GetTool<WateringCan>(this.Config.AutoRefillWateringCan.FindToolFromInventory);
        if (wateringCan is null || wateringCan.WaterLeft == wateringCan.waterCanMax) return;

        var grid = this.GetTileGrid(this.Config.AutoRefillWateringCan.Range);
        
        foreach (var tile in grid)
        {
            if (location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y))
            {
                this.UseToolOnTile(location, player, wateringCan, tile);
                break;
            }
        }
    }
}