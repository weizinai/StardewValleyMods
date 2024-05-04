using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Common;
using StardewValley.TerrainFeatures;

namespace LazyMod.Framework.Automation;

public class AutoMining : Automate
{
    private readonly ModConfig config;

    public AutoMining(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        TileCache.Clear();
        // 自动清理石头
        if (config.AutoClearStone && (tool is Pickaxe || config.FindPickaxeFromInventory)) AutoClearStone(location, player);
        // 自动收集煤炭
        if (config.AutoCollectCoal) AutoCollectCoal(location, player);
        // 自动破坏容器
        if (config.AutoBreakContainer && (tool is MeleeWeapon || config.FindWeaponFromInventory)) AutoBreakContainer(location, player);
        // 自动打开宝藏
        if (config.AutoOpenTreasure) AutoOpenTreasure(location, player);
        // 自动清理水晶
        if (config.AutoClearCrystal) AutoClearCrystal(location, player);
        TileCache.Clear();
    }

    // 自动清理石头
    private void AutoClearStone(GameLocation location, Farmer player)
    {
        if (!config.ClearStoneOnMineShaft && location is MineShaft) return;
        if (!config.ClearStoneOnVolcano && location is VolcanoDungeon) return;

        var pickaxe = FindToolFromInventory<Pickaxe>();
        if (pickaxe is null) return;

        var stoneTypes = new Dictionary<HashSet<string>, bool>
        {
            { ItemRepository.FarmStone, config.ClearFarmStone },
            { ItemRepository.OtherStone, config.ClearOtherStone },
            { ItemRepository.IslandStone, config.ClearIslandStone },
            { ItemRepository.OreStone, config.ClearOreStone },
            { ItemRepository.GemStone, config.ClearGemStone },
            { ItemRepository.GeodeStone, config.ClearGeodeStone }
        };

        var hasAddMessage = true;
        var grid = GetTileGrid(player, config.AutoClearStoneRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null) continue;
            foreach (var stoneType in stoneTypes)
            {
                if (stoneType.Value && stoneType.Key.Contains(obj.QualifiedItemId))
                {
                    if (StopAutomate(player, config.StopClearStoneStamina, ref hasAddMessage)) return;
                    UseToolOnTile(location, player, pickaxe, tile);
                    break;
                }
            }
        }

        foreach (var tile in grid)
        {
            foreach (var clump in location.resourceClumps)
            {
                if (!clump.getBoundingBox().Intersects(GetTileBoundingBox(tile))) continue;

                var clear = false;
                var requiredUpgradeLevel = Tool.stone;

                if (config.ClearMeteorite && clump.parentSheetIndex.Value == ResourceClump.meteoriteIndex)
                {
                    clear = true;
                    requiredUpgradeLevel = Tool.gold;
                }
                else
                    switch (config.ClearBoulder)
                    {
                        case true when clump.parentSheetIndex.Value == ResourceClump.boulderIndex:
                            clear = true;
                            requiredUpgradeLevel = Tool.steel;
                            break;
                        case true when ResourceClumpRepository.MineBoulder.Contains(clump.parentSheetIndex.Value):
                            clear = true;
                            requiredUpgradeLevel = Tool.stone;
                            break;
                    }

                if (clear && pickaxe.UpgradeLevel >= requiredUpgradeLevel)
                {
                    if (StopAutomate(player, config.StopClearStoneStamina, ref hasAddMessage)) return;
                    UseToolOnTile(location, player, pickaxe, tile);
                    break;
                }
            }
        }
    }

    // 自动收集煤炭
    private void AutoCollectCoal(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(player, config.AutoCollectCoalRange);
        foreach (var tile in grid)
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
                CheckTileAction(location, player, tile);
    }

    // 自动破坏容器
    private void AutoBreakContainer(GameLocation location, Farmer player)
    {
        var weapon = FindToolFromInventory<MeleeWeapon>();
        if (weapon is null) return;

        var grid = GetTileGrid(player, config.AutoBreakContainerRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is BreakableContainer)
                obj.performToolAction(weapon);
        }
    }

    // 自动打开宝藏
    private void AutoOpenTreasure(GameLocation location, Farmer player)
    {
        var grid = GetTileGrid(player, config.AutoBreakContainerRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null || obj.QualifiedItemId != "(O)-1") continue;
            obj.checkForAction(player);
        }
    }

    // 自动清理水晶
    private void AutoClearCrystal(GameLocation location, Farmer player)
    {
        var tool = FindToolFromInventory<MeleeWeapon>();
        if (tool is null) return;

        var grid = GetTileGrid(player, config.AutoClearCrystalRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)319" or "(O)320" or "(O)321")
            {
                if (obj.performToolAction(tool)) location.removeObject(tile, false);
            }
        }
    }
}