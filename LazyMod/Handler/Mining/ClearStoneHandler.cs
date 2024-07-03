using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Helper;

namespace weizinai.StardewValleyMod.LazyMod.Handler;

internal class ClearStoneHandler : BaseAutomationHandler
{
    // 农场普通石头的QualifiedItemId
    private readonly HashSet<string> farmStone = new() { "(O)343", "(O)450" };

    // 其他普通石头的QualifiedItemId
    private readonly HashSet<string> otherStone = new()
    {
        "(O)32", "(O)34", "(O)36", "(O)38", "(O)40", "(O)42", "(O)48", "(O)50", "(O)52", "(O)54", "(O)56", "(O)58", "(O)668", "(O)670", "(O)760", "(O)762",
        "(O)845", "(O)846", "(O)847"
    };

    // 姜岛石头的QualifiedItemId
    private readonly HashSet<string> islandStone = new() { "(O)25", "(O)816", "(O)817", "(O)818" };

    // 矿石石头的QualifiedItemId
    private readonly HashSet<string> oreStone = new()
    {
        "(O)95", "(O)290", "(O)751", "(O)764", "(O)765", "(O)843", "(O)844", "(O)849", "(O)850", "(O)BasicCoalNode0", "(O)BasicCoalNode1",
        "(O)VolcanoCoalNode0", "(O)VolcanoCoalNode1", "(O)VolcanoGoldNode"
    };

    // 宝石石头的QualifiedItemId
    private readonly HashSet<string> gemStone = new() { "(O)2", "(O)4", "(O)6", "(O)8", "(O)10", "(O)12", "(O)14", "(O)44", "(O)46" };

    // 晶球石头的QualifiedItemId
    private readonly HashSet<string> geodeStone = new() { "(O)75", "(O)76", "(O)77", "(O)819" };

    // 卡利三花蛋的QualifiedItemId
    private readonly HashSet<string> calicoEggStone = new() { "(O)CalicoEggStone_0", "(O)CalicoEggStone_1", "(O)CalicoEggStone_2" };

    private readonly HashSet<int> mineBoulder = new() { 752, 754, 756, 758, 148 };

    public ClearStoneHandler(ModConfig config) : base(config) { }

    public override void Apply(Item item, Farmer player, GameLocation location)
    {
        if (!this.Config.ClearStoneOnMineShaft && location is MineShaft) return;
        if (!this.Config.ClearStoneOnVolcano && location is VolcanoDungeon) return;

        var pickaxe = ToolHelper.GetTool<Pickaxe>(this.Config.AutoClearStone.FindToolFromInventory);
        if (pickaxe is null) return;

        var stoneTypes = new Dictionary<HashSet<string>, bool>
        {
            { this.farmStone, this.Config.ClearFarmStone },
            { this.otherStone, this.Config.ClearOtherStone },
            { this.islandStone, this.Config.ClearIslandStone },
            { this.oreStone, this.Config.ClearOreStone },
            { this.gemStone, this.Config.ClearGemStone },
            { this.geodeStone, this.Config.ClearGeodeStone },
            { this.calicoEggStone, this.Config.ClearCalicoEggStone }
        };

        var grid = this.GetTileGrid(this.Config.AutoClearStone.Range);
        
        foreach (var tile in grid)
        {
            if (player.Stamina <= this.Config.AutoClearStone.StopStamina) return;

            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null)
            {
                foreach (var stoneType in stoneTypes)
                {
                    if (stoneType.Value && stoneType.Key.Contains(obj.QualifiedItemId))
                    {
                        this.UseToolOnTile(location, player, pickaxe, tile);
                        break;
                    }
                }
            }

            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(this.GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (this.Config.ClearMeteorite && clump.parentSheetIndex.Value == ResourceClump.meteoriteIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.gold;
                }
                else
                    switch (this.Config.ClearBoulder)
                    {
                        case true when clump.parentSheetIndex.Value == ResourceClump.boulderIndex:
                            clear = true;
                            requiredUpgradeLevel = Tool.steel;
                            break;
                        case true when this.mineBoulder.Contains(clump.parentSheetIndex.Value):
                            clear = true;
                            requiredUpgradeLevel = Tool.stone;
                            break;
                    }

                if (clear && pickaxe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    this.UseToolOnTile(location, player, pickaxe, tile);
                    break;
                }
            }
        }
    }
}