using StardewValley;
using static weizinai.StardewValleyMod.PiCore.Constant.SItem;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class PlaceCrabPotHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: CrabPot } crabPot)
        {
            this.ForEachTile(this.Config.AutoPlaceCarbPot.Range, tile =>
            {
                this.PlaceObjectAction(crabPot, tile, player, location);
                return true;
            });
        }
    }
}