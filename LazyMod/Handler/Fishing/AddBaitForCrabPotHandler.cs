using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Fishing;

internal class AddBaitForCrabPotHandler : BaseAutomationHandler
{
    public AddBaitForCrabPotHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
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