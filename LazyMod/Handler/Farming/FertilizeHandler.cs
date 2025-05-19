using StardewValley;
using StardewValley.TerrainFeatures;
using static weizinai.StardewValleyMod.PiCore.Constant.SItem;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

public class FertilizeHandler : BaseAutomationHandler
{
    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        if (item?.Category == SObject.fertilizerCategory)
        {
            this.ForEachTile(this.Config.AutoFertilize.Range, tile =>
            {
                if (item.Stack <= 0) return false;

                location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
                switch (item.QualifiedItemId)
                {
                    // 树肥逻辑
                    case TreeFertilizer:
                    {
                        if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage)
                        {
                            if (tree.fertilize())
                            {
                                player.reduceActiveItemByOne();
                            }
                        }
                        break;
                    }
                    // 其他肥料逻辑
                    default:
                    {
                        if (terrainFeature is HoeDirt hoeDirt)
                        {
                            if (hoeDirt.plant(item.ItemId, player, true))
                            {
                                player.reduceActiveItemByOne();
                            }
                        }
                        break;
                    }
                }

                return true;
            });
        }
    }
}