using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using Common;
using Microsoft.Xna.Framework;
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
        // 自动冷却岩浆
        if (config.AutoCoolLava && (tool is WateringCan || config.FindToolForCoolLava)) AutoCoolLava(location, player);
        TileCache.Clear();
    }

    // 自动清理石头
    private void AutoClearStone(GameLocation location, Farmer player)
    {
        if (player.Stamina <= config.StopClearStoneStamina) return;
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
            { ItemRepository.GeodeStone, config.ClearGeodeStone },
            { ItemRepository.CalicoEggStone, config.ClearCalicoEggStone }
        };

        var grid = GetTileGrid(player, config.AutoClearStoneRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null)
            {
                foreach (var stoneType in stoneTypes)
                {
                    if (stoneType.Value && stoneType.Key.Contains(obj.QualifiedItemId))
                    {
                        UseToolOnTile(location, player, pickaxe, tile);
                        break;
                    }
                }
            }
            
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
    
    // 自动冷却岩浆
    private void AutoCoolLava(GameLocation location, Farmer player)
    {
        if (location is not VolcanoDungeon dungeon) return;
        var wateringCan = FindToolFromInventory<WateringCan>();
        if (wateringCan is null) return;

        var hasAddWaterMessage = true;
        var grid = GetTileGrid(player, config.AutoCoolLavaRange);
        foreach (var tile in grid)
        {
            if (wateringCan.WaterLeft <= 0)
            {
                if (!hasAddWaterMessage) Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                break;
            }

            hasAddWaterMessage = false;
            
            if (player.Stamina <= config.StopCoolLavaStamina) return;
            if (!CanCoolLave(dungeon, tile)) continue;
            UseToolOnTile(location, player, wateringCan, tile);
            if (wateringCan.WaterLeft > 0 && player.ShouldHandleAnimationSound())
                player.playNearbySoundLocal("wateringCan");
        }
    }

    private bool CanCoolLave(VolcanoDungeon dungeon, Vector2 tile)
    {
        var x = (int)tile.X;
        var y = (int)tile.Y;
        return !dungeon.CanRefillWateringCanOnTile(x,y) &&
               dungeon.isTileOnMap(tile) &&
               dungeon.waterTiles[x, y] &&
               !dungeon.cooledLavaTiles.ContainsKey(tile);
    }
}