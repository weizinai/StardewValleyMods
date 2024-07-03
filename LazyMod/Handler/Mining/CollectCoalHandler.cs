using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class CollectCoalHandler : BaseAutomationHandler
{
    public CollectCoalHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (location is not MineShaft) return;

        var grid = this.GetTileGrid(this.Config.AutoCollectCoal.Range);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
                this.CheckTileAction(location, player, tile);
        }
    }
}