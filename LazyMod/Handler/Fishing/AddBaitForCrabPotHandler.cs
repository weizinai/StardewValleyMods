using StardewValley;
using StardewValley.Objects;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class AddBaitForCrabPotHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item is SObject { Category: SObject.baitCategory } bait)
        {
            this.ForEachTile(this.Config.AutoAddBaitForCarbPot.Range, tile =>
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is CrabPot crabPot && crabPot.bait.Value == null)
                {
                    if (obj.performObjectDropInAction(bait, false, player))
                    {
                        player.reduceActiveItemByOne();
                    }
                }
                return true;
            });
        }
    }
}