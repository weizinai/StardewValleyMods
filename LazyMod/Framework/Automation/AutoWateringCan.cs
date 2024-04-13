using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoWateringCan : Automate
{
    private readonly ModConfig config;

    public AutoWateringCan(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoWaterDirt && tool is WateringCan wateringCan)
            AutoWaterDirt(location, player, wateringCan);
        if (config.AutoRefillWateringCan && (tool is WateringCan || config.FindWateringCanFromInventory))
            AutoRefillWateringCan(location, player);
    }

    // 自动浇水
    private void AutoWaterDirt(GameLocation location, Farmer player, WateringCan wateringCan)
    {
        var hasAddStaminaMessage = true;
        var hasAddWaterMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoWaterDirtRange).ToList();
        foreach (var tile in grid)
        {
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            if (tileFeature is HoeDirt hoeDirt && hoeDirt.state.Value == HoeDirt.dry)
            {
                if (wateringCan.WaterLeft <= 0)
                {
                    if (!hasAddWaterMessage)
                        Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335");
                    break;
                }

                hasAddWaterMessage = false;

                if (StopAutomate(player, config.StopAutoWaterDirtStamina, ref hasAddStaminaMessage)) break;
                UseToolOnTile(location, player, wateringCan, tile);
            }
        }
    }

    // 自动填充水壶
    private void AutoRefillWateringCan(GameLocation location, Farmer player)
    {
        var wateringCan = FindToolFromInventory<WateringCan>();
        if (wateringCan is null || wateringCan.WaterLeft == wateringCan.waterCanMax)
            return;

        var origin = Game1.player.Tile;
        var grid = GetTileGrid(origin, config.AutoRefillWateringCanRange).ToList();
        foreach (var tile in grid.Where(tile => location.CanRefillWateringCanOnTile((int)tile.X, (int)tile.Y)))
        {
            UseToolOnTile(location, player, wateringCan, tile);
            break;
        }
    }
}