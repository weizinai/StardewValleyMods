using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class PlaceFloorHandler : BaseAutomationHandler
{
    public PlaceFloorHandler(ModConfig config) : base(config) { }

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