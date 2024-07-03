using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PlaceFloorHandler : BaseAutomationHandler
{
    public PlaceFloorHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item is SObject floor && floor.IsFloorPathItem())
        {
            var grid = this.GetTileGrid(this.Config.AutoPlaceFloor.Range);
            
            foreach (var tile in grid) this.PlaceObjectAction(floor, tile, player, location);
        }
    }
}