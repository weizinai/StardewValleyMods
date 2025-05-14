using StardewValley;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class DigSpotHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var hoe = ToolHelper.GetTool<Hoe>(this.Config.AutoDigSpots.FindToolFromInventory);
        if (hoe is null) return;

        this.ForEachTile(this.Config.AutoDigSpots.Range, tile =>
        {
            if (player.Stamina <= this.Config.AutoDigSpots.StopStamina) return false;

            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)590" or "(O)SeedSpot") this.UseToolOnTile(location, player, hoe, tile);
            return true;
        });
    }
}