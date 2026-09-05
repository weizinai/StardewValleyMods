using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ChopTreeHandler : BaseAutomationHandler
{
    public ChopTreeHandler(ModConfig config) : base(config) { }

    public override bool IsEnable()
    {
        return base.IsEnable() && Game1.currentLocation is not Town;
    }

    public override void Apply(Item? item, Farmer player, GameLocation location)
    {
        var axe = ToolHelper.GetTool<Axe>(this.config.AutoChopTree.FindToolFromInventory);

        if (axe is null)
        {
            return;
        }

        var treeType = new Dictionary<string, Dictionary<int, bool>>
        {
            { Tree.bushyTree, this.config.ChopOakTree },
            { Tree.leafyTree, this.config.ChopMapleTree },
            { Tree.pineTree, this.config.ChopPineTree },
            { Tree.mahoganyTree, this.config.ChopMahoganyTree },
            { Tree.palmTree, this.config.ChopPalmTree },
            { Tree.palmTree2, this.config.ChopPalmTree },
            { Tree.mushroomTree, this.config.ChopMushroomTree },
            { Tree.greenRainTreeBushy, this.config.ChopGreenRainTree },
            { Tree.greenRainTreeLeafy, this.config.ChopGreenRainTree },
            { Tree.greenRainTreeFern, this.config.ChopGreenRainTree },
            { Tree.mysticTree, this.config.ChopMysticTree }
        };

        this.ForEachTile(this.config.AutoChopTree.Range, tile =>
        {
            if (player.Stamina <= this.config.AutoChopTree.StopStamina)
            {
                return false;
            }

            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);

            if (terrainFeature is Tree tree)
            {
                if (tree.tapped.Value || tree.stopGrowingMoss.Value)
                {
                    return true;
                }

                foreach (var (key, value) in treeType)
                {
                    if (tree.treeType.Value == key)
                    {
                        foreach (var (stage, chopTree) in value)
                        {
                            if (tree.growthStage.Value < 5 && tree.growthStage.Value == stage && chopTree ||
                                tree.growthStage.Value >= 5 && !tree.stump.Value && value[5] ||
                                tree.stump.Value && value[-1])
                            {
                                this.UseToolOnTile(location, player, axe, tile);

                                break;
                            }
                        }

                        break;
                    }
                }
            }

            return true;
        });
    }
}
