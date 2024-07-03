using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ChopTreeHandler : BaseAutomationHandler
{
    public ChopTreeHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        var axe = ToolHelper.GetTool<Axe>(this.Config.AutoChopTree.FindToolFromInventory);
        if (axe is null) return;

        var treeType = new Dictionary<string, Dictionary<int, bool>>
        {
            { Tree.bushyTree, this.Config.ChopOakTree },
            { Tree.leafyTree, this.Config.ChopMapleTree },
            { Tree.pineTree, this.Config.ChopPineTree },
            { Tree.mahoganyTree, this.Config.ChopMahoganyTree },
            { Tree.palmTree, this.Config.ChopPalmTree },
            { Tree.palmTree2, this.Config.ChopPalmTree },
            { Tree.mushroomTree, this.Config.ChopMushroomTree },
            { Tree.greenRainTreeBushy, this.Config.ChopGreenRainTree },
            { Tree.greenRainTreeLeafy, this.Config.ChopGreenRainTree },
            { Tree.greenRainTreeFern, this.Config.ChopGreenRainTree },
            { Tree.mysticTree, this.Config.ChopMysticTree }
        };

        var grid = this.GetTileGrid(this.Config.AutoChopTree.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoChopTree.StopStamina) return;

            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree)
            {
                if (tree.tapped.Value && !this.Config.ChopTapperTree) continue;
                if (tree.stopGrowingMoss.Value && !this.Config.ChopVinegarTree) continue;

                foreach (var (key, value) in treeType)
                {
                    // 树逻辑
                    if (tree.treeType.Value.Equals(key))
                    {
                        foreach (var (stage, chopTree) in value)
                        {
                            if (tree.growthStage.Value < 5 && tree.growthStage.Value == stage && chopTree ||
                                tree.growthStage.Value >= 5 && !tree.stump.Value && value[5])
                            {
                                this.UseToolOnTile(location, player, axe, tile);
                                break;
                            }
                        }

                        break;
                    }

                    // 树桩逻辑
                    if (tree.stump.Value && value[-1])
                    {
                        this.UseToolOnTile(location, player, axe, tile);
                        break;
                    }
                }
            }
        }
    }
}