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
        if (config.AutoSeed && item?.Category == SObject.SeedsCategory)
            AutoSeed(location, player, item);
        if (config.AutoFertilize && item?.Category == SObject.fertilizerCategory)
            AutoFertilize(location, player, item);
        if (config.AutoHarvestCrop)
            AutoHarvestCrop(location, player);
    }

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
                case "(O)805":
                    if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage &&
                        tree.fertilize())
                        ConsumeItem(player, item);
                    break;
                default:
                    if (terrainFeature is HoeDirt hoeDirt && hoeDirt.plant(item.ItemId, player, true))
                        ConsumeItem(player, item);
                    break;
            }
        }
    }

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
                if (ItemRegistry.GetData(crop.indexOfHarvest.Value).Category == SObject.flowersCategory)
                    continue;
                if (crop.harvest((int)tile.X, (int)tile.Y, hoeDirt))
                {
                    hoeDirt.destroyCrop(true);
                    if (location is IslandLocation && Game1.random.NextDouble() < 0.05)
                        player.team.RequestLimitedNutDrops("IslandFarming", location, (int)tile.X * 64, (int)tile.Y * 64, 5);
                }
            }
        }
    }
}