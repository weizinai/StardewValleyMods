using StardewValley;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoFishing : Automate
{
    private readonly ModConfig config;

    public AutoFishing(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (location is null) return;

        TileCache.Clear();
        // 自动使用蟹笼
        if (config.AutoPlaceCarbPot && item is SObject { QualifiedItemId: "(O)710" } crabPot) AutoUseCrabPot(location, player, crabPot);
        TileCache.Clear();
    }

    // 自动使用蟹笼
    private void AutoUseCrabPot(GameLocation location, Farmer player, SObject crabPot)
    {
        var grid = GetTileGrid(player, config.AutoPlaceCarbPotRange);
        foreach (var _ in grid.Select(tile => GetTilePixelPosition(tile))
                     .Where(tilePixelPosition => crabPot.placementAction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, player)))
        {
            ConsumeItem(player, crabPot);
        }
    }
}