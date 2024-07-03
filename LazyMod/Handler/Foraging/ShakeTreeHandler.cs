using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ShakeTreeHandler : BaseAutomationHandler
{
    public ShakeTreeHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoShakeTree.Range);
        
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasSeed.Value)
                tree.performUseAction(tile);
        }
    }
}