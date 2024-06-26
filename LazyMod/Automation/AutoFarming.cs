﻿using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoFarming : Automate
{
    public AutoFarming(ModConfig config, Func<int, List<Vector2>> getTileGrid) : base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        // 自动耕地
        if (this.Config.AutoTillDirt.IsEnable && tool is Hoe) this.AutoTillDirt(location, player, tool);
        // 自动清理耕地
        if (this.Config.AutoClearTilledDirt.IsEnable && tool is Pickaxe) this.AutoClearTilledDirt(location, player, tool);
        // 自动浇水
        if (this.Config.AutoWaterDirt.IsEnable && tool is WateringCan wateringCan) this.AutoWaterDirt(location, player, wateringCan);
        // 自动填充水壶
        if (this.Config.AutoRefillWateringCan.IsEnable && (tool is WateringCan || this.Config.AutoRefillWateringCan.FindToolFromInventory)) this.AutoRefillWateringCan(location, player);
        // 自动播种
        if (this.Config.AutoSeed.IsEnable && item?.Category == SObject.SeedsCategory) this.AutoSeed(location, player, item);
        // 自动施肥
        if (this.Config.AutoFertilize.IsEnable && item?.Category == SObject.fertilizerCategory) this.AutoFertilize(location, player, item);
        // 自动收获作物
        if (this.Config.AutoHarvestCrop.IsEnable) this.AutoHarvestCrop(location, player);
        // 自动摇晃果树
        if (this.Config.AutoShakeFruitTree.IsEnable) this.AutoShakeFruitTree(location);
        // 自动清理枯萎作物
        if (this.Config.AutoClearDeadCrop.IsEnable && (tool is MeleeWeapon || this.Config.AutoClearDeadCrop.FindToolFromInventory)) this.AutoClearDeadCrop(location);
    }

    // 自动耕地
    private void AutoTillDirt(GameLocation location, Farmer player, Tool tool)
    {
        if (player.Stamina <= this.Config.AutoTillDirt.StopStamina) return;

        var grid = this.GetTileGrid(this.Config.AutoTillDirt.Range);
        foreach (var tile in grid)
        {
            if (!this.CanTillDirt(location, tile)) continue;
            this.UseToolOnTile(location, player, tool, tile);
        }
    }

    // 自动清理耕地
    private void AutoClearTilledDirt(GameLocation location, Farmer player, Tool tool)
    {
        if (player.Stamina <= this.Config.AutoClearTilledDirt.StopStamina) return;

        var grid = this.GetTileGrid(this.Config.AutoClearTilledDirt.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt { crop: null } hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                this.UseToolOnTile(location, player, tool, tile);
            }
        }
    }

    // 自动浇水
    private void AutoWaterDirt(GameLocation location, Farmer player, WateringCan wateringCan)
    {
        if (player.Stamina <= this.Config.AutoWaterDirt.StopStamina) return;

        var hasAddWaterMessage = true;
        var grid = this.GetTileGrid(this.Config.AutoWaterDirt.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                if (wateringCan.WaterLeft <= 0)
                {
                    if (!hasAddWaterMessage) Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                    break;
                }

                hasAddWaterMessage = false;

                this.UseToolOnTile(location, player, wateringCan, tile);
                if (wateringCan.WaterLeft > 0 && player.ShouldHandleAnimationSound())
                {
                    player.playNearbySoundLocal("wateringCan");
                }
            }
        }
    }

    // 自动填充水壶
    private void AutoRefillWateringCan(GameLocation location, Farmer player)
    {
        var wateringCan = this.FindToolFromInventory<WateringCan>();
        if (wateringCan is null || wateringCan.WaterLeft == wateringCan.waterCanMax)
            return;

        var grid = this.GetTileGrid(this.Config.AutoRefillWateringCan.Range);
        foreach (var tile in grid.Where(tile => location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y)))
        {
            this.UseToolOnTile(location, player, wateringCan, tile);
            break;
        }
    }

    // 自动播种
    private void AutoSeed(GameLocation location, Farmer player, Item item)
    {
        var grid = this.GetTileGrid(this.Config.AutoSeed.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: null } hoeDirt)
            {
                location.objects.TryGetValue(tile, out var obj);
                if (obj is not null) continue;

                if (item.Stack <= 0)
                    break;

                if (hoeDirt.plant(item.ItemId, player, false)) player.reduceActiveItemByOne();
            }
        }
    }

    // 自动施肥
    private void AutoFertilize(GameLocation location, Farmer player, Item item)
    {
        var grid = this.GetTileGrid(this.Config.AutoFertilize.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (item.Stack <= 0)
                break;
            switch (item.QualifiedItemId)
            {
                // 树肥逻辑
                case "(O)805":
                    if (terrainFeature is Tree tree && !tree.fertilized.Value && tree.growthStage.Value < Tree.treeStage && tree.fertilize())
                        player.reduceActiveItemByOne();
                    break;
                // 其他肥料逻辑
                default:
                    if (terrainFeature is HoeDirt hoeDirt && hoeDirt.plant(item.ItemId, player, true))
                        player.reduceActiveItemByOne();
                    break;
            }
        }
    }

    // 自动收获作物
    private void AutoHarvestCrop(GameLocation location, Farmer player)
    {
        var grid = this.GetTileGrid(this.Config.AutoHarvestCrop.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                // 自动收获花逻辑
                if (!this.Config.AutoHarvestFlower && ItemRegistry.GetData(crop.indexOfHarvest.Value)?.Category == SObject.flowersCategory)
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
    private void AutoShakeFruitTree(GameLocation location)
    {
        var grid = this.GetTileGrid(this.Config.AutoShakeFruitTree.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is FruitTree fruitTree && fruitTree.fruit.Count > 0)
                fruitTree.performUseAction(tile);
        }
    }

    // 自动清理枯萎作物
    private void AutoClearDeadCrop(GameLocation location)
    {
        var scythe = this.FindToolFromInventory<MeleeWeapon>();
        if (scythe is null) return;

        var grid = this.GetTileGrid(this.Config.AutoHarvestCrop.Range);
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var terrainFeature);
            if (terrainFeature is HoeDirt { crop: not null } hoeDirt)
            {
                var crop = hoeDirt.crop;
                if (crop.dead.Value) hoeDirt.performToolAction(scythe, 0, tile);
            }
        }
    }

    private bool CanTillDirt(GameLocation location, Vector2 tile)
    {
        location.terrainFeatures.TryGetValue(tile, out var tileFeature);
        location.objects.TryGetValue(tile, out var obj);
        return tileFeature is null && obj is null &&
               !location.IsTileOccupiedBy(tile, CollisionMask.All, CollisionMask.Farmers) &&
               location.isTilePassable(tile) &&
               location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is not null;
    }
}