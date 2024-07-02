using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler.Foraging;

internal class PlaceTapperHandler : BaseAutomationHandler
{
    public PlaceTapperHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
        if (item is SObject { QualifiedItemId: "(BC)105" or "(BC)264" } tapper)
        {
            var grid = this.GetTileGrid(this.Config.AutoPlaceVinegar.Range);
            foreach (var tile in grid)
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is Tree tree && !tree.tapped.Value)
                {
                    var tilePixelPosition = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
                    if (tapper.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)) player.reduceActiveItemByOne();
                }
            }
        }
    }
}