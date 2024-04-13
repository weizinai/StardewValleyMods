using StardewValley;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoShears : Automate
{
    private readonly ModConfig config;

    public AutoShears(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoShearsAnimal && (tool is Shears || config.FindShearsFromInventory))
            AutoShearsAnimal(location, player);
    }

    private void AutoShearsAnimal(GameLocation location, Farmer player)
    {
        var shears = FindToolFromInventory<Shears>();
        if (shears is null)
            return;
        
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoShearsAnimalRange);
        foreach (var tile in grid)
        {
            var animal = GetBestHarvestableFarmAnimal(location, shears, tile);
            if (animal is null)
                break;
            shears.animal = animal;
            UseToolOnTile(location, player, shears, tile);
        }
    }
}