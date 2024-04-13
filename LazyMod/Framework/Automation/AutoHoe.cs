using StardewValley;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoHoe : Automate
{
    private readonly ModConfig config;

    public AutoHoe(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoTillDirt && tool is Hoe)
            AutoTillDirt(location, player, tool);
        if (config.AutoDigArtifactSpots && (tool is Hoe || config.FindHoeFromInventory))
            AutoDigArtifactSpots(location, player);
    }

    // 自动耕地
    private void AutoTillDirt(GameLocation location, Farmer player, Tool tool)
    {
        var hasAddMessage = true;
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoTillDirtRange).ToList();
        foreach (var tile in grid)
        {
            // 如果该瓦片不可耕地,则跳过该瓦片的处理
            location.terrainFeatures.TryGetValue(tile, out var tileFeature);
            location.objects.TryGetValue(tile, out var obj);
            if (tileFeature is not null || obj is not null || location.IsTileOccupiedBy(tile, CollisionMask.All, CollisionMask.Farmers) ||
                !location.isTilePassable(tile) || location.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Diggable", "Back") is null)
                continue;

            if (StopAutomate(player, config.StopAutoTillDirtStamina, ref hasAddMessage)) break;
            UseToolOnTile(location, player, tool, tile);
        }
    }

    // 自动挖掘远古斑点
    private void AutoDigArtifactSpots(GameLocation location, Farmer player)
    {
        var hoe = FindToolFromInventory<Hoe>();
        if (hoe is null)
            return;

        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoDigArtifactSpotsRange).ToList();
        var hasAddMessage = true;
        foreach (var tile in grid)
        {
            location.objects.TryGetValue(tile, out var obj);
            if (obj?.QualifiedItemId == "(O)590")
            {
                if (StopAutomate(player, config.StopAutoTillDirtStamina, ref hasAddMessage)) break;
                hoe.DoFunction(location, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), 1, player);
            }
        }
    }
}