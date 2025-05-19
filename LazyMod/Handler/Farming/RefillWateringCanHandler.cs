using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class RefillWateringCanHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var wateringCan = ToolHelper.GetTool<WateringCan>(this.Config.AutoRefillWateringCan.FindToolFromInventory);

        if (wateringCan == null || wateringCan.WaterLeft == wateringCan.waterCanMax) return;

        this.ForEachTile(this.Config.AutoRefillWateringCan.Range, tile =>
        {
            if (location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y))
            {
                this.UseToolOnTile(location, player, wateringCan, tile);
                return false;
            }

            return true;
        });
    }
}