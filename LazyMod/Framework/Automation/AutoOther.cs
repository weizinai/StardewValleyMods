using StardewValley;
using StardewValley.Tools;

namespace LazyMod.Framework.Automation;

public class AutoOther : Automate
{
    private readonly ModConfig config;

    public AutoOther(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;
        

        if (config.AutoDigArtifactSpots && (tool is Hoe || config.FindHoeFromInventory))
            AutoDigArtifactSpots(location, player);
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
                UseToolOnTile(location, player, hoe, tile);
            }
        }
    }
}