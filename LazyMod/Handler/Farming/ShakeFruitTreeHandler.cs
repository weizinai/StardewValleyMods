using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ShakeFruitTreeHandler : BaseAutomationHandler
{
    public ShakeFruitTreeHandler(ModConfig config) : base(config) { }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        this.ForEachTile(this.Config.AutoShakeFruitTree.Range, tile =>
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0) fruitTree.performUseAction(tile);
            return true;
        });
    }
}