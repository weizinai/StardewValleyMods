using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ShakeFruitTreeHandler : BaseAutomationHandler
{
    public ShakeFruitTreeHandler(ModConfig config) : base(config) { }

    public override void Apply(Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoShakeFruitTree.Range);

        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0)
                fruitTree.performUseAction(tile);
        }
    }
}