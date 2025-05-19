using StardewValley;
using StardewValley.Locations;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class CollectCoalHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (location is not MineShaft) return;

        this.ForEachTile(this.Config.AutoCollectCoal.Range, tile =>
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
            {
                this.CheckTileAction(location, player, tile);
            }
            return true;
        });
    }
}