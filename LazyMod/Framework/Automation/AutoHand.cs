using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoHand : Automate
{
    private readonly ModConfig config;

    public AutoHand(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动播种
        if (config.AutoSeed && item?.Category == SObject.SeedsCategory) AutoSeed(location, player, item);
        // 自动施肥
        if (config.AutoFertilize && item?.Category == SObject.fertilizerCategory) AutoFertilize(location, player, item);
        // 自动收获作物
        if (config.AutoHarvestCrop) AutoHarvestCrop(location, player);
        // 自动摇晃果树
        if (config.AutoShakeFruitTree) AutoShakeFruitTree(location, player);
        // 自动抚摸动物
        if (config.AutoPetAnimal) AutoPetAnimal(location, player);
        // 自动抚摸宠物
        if (config.AutoPetPet) AutoPetPet(location, player);
    }

    // 自动播种
    private void AutoSeed(GameLocation location, Farmer player, Item item)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoSeedRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: null } hoeDirt)
            {
                if (item.Stack <= 0)
                    break;

                if (hoeDirt.plant(item.ItemId, player, false))
                    ConsumeItem(player, item);
            }
        }
    }

    // 自动施肥
    private void AutoFertilize(GameLocation location, Farmer player, Item item)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoFertilizeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (item.Stack <= 0)
                break;
            switch (item.QualifiedItemId)
            {
                // 树肥逻辑
                case "(O)805":
                    if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage &&
                        tree.fertilize())
                        ConsumeItem(player, item);
                    break;
                // 其他肥料逻辑
                default:
                    if (terrainFeature is HoeDirt hoeDirt && hoeDirt.plant(item.ItemId, player, true))
                        ConsumeItem(player, item);
                    break;
            }
        }
    }

    // 自动收获作物
    private void AutoHarvestCrop(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoHarvestCropRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                // 自动收获花逻辑
                if (!config.AutoHarvestFlower && ItemRegistry.GetData(crop.indexOfHarvest.Value).Category == SObject.flowersCategory)
                    continue;
                if (crop.harvest((int)tile.X, (int)tile.Y, hoeDirt))
                {
                    hoeDirt.destroyCrop(true);
                    // 姜岛金核桃逻辑
                    if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
                        player.team.RequestLimitedNutDrops("IslandFarming", location, (int)tile.X * 64, (int)tile.Y * 64, 5);
                }
            }
        }
    }

    // 自动摇晃果树
    private void AutoShakeFruitTree(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShakeFruitTreeRange);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0)
                fruitTree.performUseAction(tile);
        }
    }

    // 自动抚摸动物
    private void AutoPetAnimal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoPetAnimalRange).ToList();

        var animals = location.animals.Values;
        foreach (var animal in animals)
        {
            foreach (var tile in grid)
            {
                var tilePixelPosition = GetTilePixelPosition(tile, false);
                if (animal.GetCursorPetBoundingBox().Intersects(new Rectangle((int)tilePixelPosition.X, (int)tilePixelPosition.Y, 
                        Game1.tileSize, Game1.tileSize)) && !animal.wasPet.Value)
                    animal.pet(player);
            }
        }
    }

    private void AutoPetPet(GameLocation location, Farmer player)
    {
        
    }
}