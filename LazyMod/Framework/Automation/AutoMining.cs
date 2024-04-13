using StardewValley;
using xTile.Dimensions;

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
        
        // 自动收集煤炭
        if (config.AutoCollectCoal) AutoCollectCoal(location, player);
    }

    // 自动收集煤炭
    private void AutoCollectCoal(GameLocation location, Farmer player)
    {
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoCollectCoalRange);
        foreach (var tile in grid)
        {
            if (location.getTileIndexAt(new Location((int)tile.X, (int)tile.Y), "Buildings") == 194)
                CheckTileAction(location, player, tile);
        }
    }
}