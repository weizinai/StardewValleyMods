using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Foraging;

internal class PlaceVinegarHandler : BaseAutomationHandler
{
    public PlaceVinegarHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: "(O)419" } vinegar)
        {
            var grid = this.GetTileGrid(this.Config.AutoPlaceVinegar.Range);
            
            foreach (var tile in grid)
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is Tree tree && !tree.stopGrowingMoss.Value)
                {
                    var tilePixelPosition = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
                    if (vinegar.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
                }
            }
        }
    }
}