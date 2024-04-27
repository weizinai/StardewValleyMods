using StardewValley;
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
            { new HashSet<string> { "343", "450" }, config.ClearFarmStone },
            {
                new HashSet<string> { "32", "34", "36", "38", "40", "42", "48", "50", "52", "54", "56", "58", "668", "670", "760", "762", "845", "846", "847" },
                config.ClearOtherStone
            },
            { new HashSet<string> { "25", "816", "817", "818" }, config.ClearIslandStone },
            {
                new HashSet<string>
                {
                    "95", "290", "751", "764", "765", "843", "844", "849", "850", "BasicCoalNode0", "BasicCoalNode1", "VolcanoCoalNode0", "VolcanoCoalNode1",
                    "VolcanoGoldNode"
                },
                config.ClearOreStone
            },
            { new HashSet<string> { "2", "4", "6", "8", "10", "12", "14", "44", "46" }, config.ClearGemStone },
            { new HashSet<string> { "75", "76", "77", "819" }, config.ClearGeodeStone }
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
                if (stoneType.Value && stoneType.Key.Contains(obj.ItemId))
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