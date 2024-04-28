﻿using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoMining : Automate
{
    private readonly ModConfig config;

    public AutoMining(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

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
            { new HashSet<string> { "(O)343", "(O)450" }, config.ClearFarmStone },
            {
                new HashSet<string>
                {
                    "(O)32", "(O)34", "(O)36", "(O)38", "(O)40", "(O)42", "(O)48", "(O)50", "(O)52", "(O)54", "(O)56", "(O)58", "(O)668", "(O)670", "(O)760", "(O)762",
                    "(O)845", "(O)846", "(O)847"
                },
                config.ClearOtherStone
            },
            { new HashSet<string> { "(O)25", "(O)816", "(O)817", "(O)818" }, config.ClearIslandStone },
            {
                new HashSet<string>
                {
                    "(O)95", "(O)290", "(O)751", "(O)764", "(O)765", "(O)843", "(O)844", "(O)849", "(O)850", "(O)BasicCoalNode0", "(O)BasicCoalNode1",
                    "(O)VolcanoCoalNode0", "(O)VolcanoCoalNode1", "(O)VolcanoGoldNode"
                },
                config.ClearOreStone
            },
            { new HashSet<string> { "(O)2", "(O)4", "(O)6", "(O)8", "(O)10", "(O)12", "(O)14", "(O)44", "(O)46" }, config.ClearGemStone },
            { new HashSet<string> { "(O)75", "(O)76", "(O)77", "(O)819" }, config.ClearGeodeStone }
        };

        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearStoneRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is null) continue;

            foreach (var stoneType in stoneTypes)
            {
                if (stoneType.Value && stoneType.Key.Contains(obj.QualifiedItemId))
                {
                    if (StopAutomate(player, config.StopAutoClearStoneStamina, ref hasAddMessage)) return;
                    UseToolOnTile(location, player, pickaxe, tile);
                    break;
                }
            }
        }
    }

    // 自动收集煤炭
    private void AutoCollectCoal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoCollectCoalRange);
        foreach (var tile in grid)
            if (location.getTileIndexAt((int)tile.X, (int)tile.Y, "Buildings") == 194)
                CheckTileAction(location, player, tile);
    }

    // 自动破坏容器
    private void AutoBreakContainer(GameLocation location, Farmer player)
    {
        var weapon = FindToolFromInventory<MeleeWeapon>();
        if (weapon is null) return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoBreakContainerRange);
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
        // if (location is not MineShaft) return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoBreakContainerRange);
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
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearCrystalRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId is "(O)319" or "(O)320" or "(O)321")
            {
                obj.performToolAction(FakeScythe.Value);
                location.removeObject(tile, false);
            }
        }
    }
}