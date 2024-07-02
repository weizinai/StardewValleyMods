using StardewValley;
using StardewValley.TerrainFeatures;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class FertilizeHandler : BaseAutomationHandler
{
    public FertilizeHandler(ModConfig config) : base(config) { }
    
    public override void Apply(Farmer player, GameLocation location)
    {
        var item = player.CurrentItem;
        if (item is null || item.Category != SObject.fertilizerCategory) return;
        
        var grid = this.GetTileGrid(this.Config.AutoFertilize.Range);
        
        foreach (var tile in grid)
        {
            if (item.Stack <= 0) break;
            
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            switch (item.QualifiedItemId)
            {
                // 树肥逻辑
                case "(O)805":
                    if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage && tree.fertilize())
                        player.reduceActiveItemByOne();
                    break;
                // 其他肥料逻辑
                default:
                    if (terrainFeature is HoeDirt hoeDirt && hoeDirt.plant(item.ItemId, player, true))
                        player.reduceActiveItemByOne();
                    break;
            }
        }
    }
}