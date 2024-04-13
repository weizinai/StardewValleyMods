using StardewValley;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoMailPail : Automate
{
    private readonly ModConfig config;

    public AutoMailPail(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (config.AutoMilkAnimal && (tool is MilkPail || config.FindMilkPailFromInventory))
            AutoMilkAnimal(location, player);
    }
    
    private void AutoMilkAnimal(GameLocation location, Farmer player)
    {
        var milkPail = FindToolFromInventory<MilkPail>();
        if (milkPail is null) return;
        
        var origin = player.Tile;
        var grid = GetTileGrid(origin, config.AutoMilkAnimalRange);
        foreach (var tile in grid)
        {
            var animal = GetBestHarvestableFarmAnimal(location, milkPail, tile);
            if (animal is null)
                break;
            UseToolOnTile(location, player, milkPail, tile);
        }
    }
}