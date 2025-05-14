using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class PlaceCrabPotHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: "(O)710" } crabPot)
        {
            this.ForEachTile(this.Config.AutoPlaceCarbPot.Range, tile =>
            {
                this.PlaceObjectAction(crabPot, tile, player, location);
                return true;
            });
        }
    }
}