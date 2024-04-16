using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoForaging : Automate
{
    private readonly ModConfig config;

    public AutoForaging(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

        // 自动觅食
        if (config.AutoForage) AutoForage(location, player);
        // 自动摇树
        if (config.AutoShakeTree) AutoShakeTree(location, player);
        // 自动收获苔藓
        if (config.AutoHarvestMoss && (tool is MeleeWeapon || config.FindScytheFromInventory)) AutoHarvestMoss(location, player);
        // 自动清理树枝
        if (config.AutoClearTwig && (tool is Axe || config.FindAxeFromInventory)) AutoClearTwig(location, player);
        // 自动清理树种
        if (config.AutoClearTreeSeed && tool is Axe) AutoClearTreeSeed(location, player, tool);
    }

    // 自动觅食
    private void AutoForage(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoForageRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsSpawnedObject)
                CheckTileAction(location, player, tile);

            foreach (var terrainFeature in location.largeTerrainFeatures)
                if (terrainFeature is Bush bush && bush.getBoundingBox().Intersects(GetTileBoundingBox(tile)) &&
                    bush.tileSheetOffset.Value == 1 && bush.size.Value == Bush.mediumBush && !bush.townBush.Value)
                    bush.performUseAction(tile);
        }
    }

    // 自动摇树
    private void AutoShakeTree(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShakeFruitTreeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasSeed.Value)
                tree.performUseAction(tile);
        }
    }

    // 自动收获苔藓
    private void AutoHarvestMoss(GameLocation location, Farmer player)
    {
        var scythe = FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestMossRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.hasMoss.Value)
                tree.performToolAction(scythe, 0, tile);
        }
    }

    // 自动清理树枝
    private void AutoClearTwig(GameLocation location, Farmer player)
    {
        var axe = FindToolFromInventory<Axe>();
        if (axe is null) return;

        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearTreeSeedRange);
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj is not null && obj.IsTwig())
            {
                if (StopAutomate(player, config.StopAutoClearTreeSeedStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, axe, tile);
            }
        }
    }

    // 自动清理树种
    private void AutoClearTreeSeed(GameLocation location, Farmer player, Tool tool)
    {
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoClearTreeSeedRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is Tree tree && tree.growthStage.Value == Tree.seedStage)
            {
                if (StopAutomate(player, config.StopAutoClearTreeSeedStamina, ref hasAddMessage)) break;
                UseToolOnTile(location, player, tool, tile);
            }
        }
    }
}