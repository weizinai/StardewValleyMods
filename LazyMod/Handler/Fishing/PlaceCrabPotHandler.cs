using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Fishing;

internal class PlaceCrabPotHandler : BaseAutomationHandler
{
    public PlaceCrabPotHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
        if (item is SObject { QualifiedItemId: "(O)710" } crabPot)
        {
            var grid = this.GetTileGrid(this.Config.AutoPlaceCarbPot.Range);

            foreach (var tile in grid)
            {
                var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
                if (crabPot.placementAction(location, (int)position.X, (int)position.Y, player))
                {
                    player.reduceActiveItemByOne();
                }
            }
        }
    }
}