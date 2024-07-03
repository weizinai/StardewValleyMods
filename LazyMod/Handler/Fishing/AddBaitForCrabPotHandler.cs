using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class AddBaitForCrabPotHandler : BaseAutomationHandler
{
    public AddBaitForCrabPotHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item is SObject { Category: SObject.baitCategory } bait)
        {
            var grid = this.GetTileGrid(this.Config.AutoAddBaitForCarbPot.Range);
            foreach (var tile in grid)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not CrabPot crabPot || crabPot.bait.Value is not null) continue;
                if (obj.performObjectDropInAction(bait, false, player))
                    player.reduceActiveItemByOne();
            }
        }
    }
}