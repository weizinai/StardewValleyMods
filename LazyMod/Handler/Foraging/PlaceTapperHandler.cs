using StardewValley;
using StardewValley.TerrainFeatures;
using static weizinai.StardewValleyMod.PiCore.Constant.SItem;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class PlaceTapperHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item is SObject { QualifiedItemId: Tapper or HeavyTapper } tapper)
        {
            this.ForEachTile(this.Config.AutoPlaceTapper.Range, tile =>
            {
                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                if (terrainFeature is Tree tree && !tree.tapped.Value)
                {
                    this.PlaceObjectAction(tapper, tile, player, location);
                }
                return true;
            });
        }
    }
}