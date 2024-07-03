using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Other;

internal class PlaceFloorHandler : BaseAutomationHandler
{
    public PlaceFloorHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
        if (item is SObject floor && floor.IsFloorPathItem())
        {
            var grid = this.GetTileGrid(this.Config.AutoPlaceFloor.Range);
            foreach (var tile in grid)
            {
                var tilePixelPosition = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
                if (floor.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
            }
        }
    }
}