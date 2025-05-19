using StardewValley;
using StardewValley.TerrainFeatures;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class ShakeFruitTreeHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoShakeFruitTree.Range, tile =>
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0)
            {
                fruitTree.performUseAction(tile);
            }
            return true;
        });
    }
}