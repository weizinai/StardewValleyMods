using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class PlaceFloorHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item is SObject floor && floor.IsFloorPathItem())
        {
            this.ForEachTile(this.Config.AutoPlaceFloor.Range, tile =>
            {
                this.PlaceObjectAction(floor, tile, player, location);
                return true;
            });
        }
    }
}