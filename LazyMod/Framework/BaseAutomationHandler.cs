using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;

namespace weizinai.StardewValleyMod.LazyMod.Framework;

internal abstract class BaseAutomationHandler : IAutomationHandler
{
    protected readonly ModConfig Config;
    protected readonly Func<int, List<Vector2>> GetTileGrid = TileHelper.GetTileGrid;

    protected BaseAutomationHandler(ModConfig config)
    {
        this.Config = config;
    }
    
    public abstract void Apply(Farmer player, GameLocation location);

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile);
        return new Rectangle((int)position.X, (int)position.Y, Game1.tileSize, Game1.tileSize);
    }
    
    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile, true);
        tool.swingTicker++;
        tool.DoFunction(location, (int)position.X, (int)position.Y, 1, player);
    }
    
    protected FarmAnimal? GetBestHarvestableFarmAnimal(Tool tool, Vector2 tile, IEnumerable<FarmAnimal> animals)
    {
        var animal = Utility.GetBestHarvestableFarmAnimal(animals, tool, this.GetTileBoundingBox(tile));
        if (animal?.currentProduce.Value is null || animal.isBaby() || !animal.CanGetProduceWithTool(tool))
            return null;

        return animal;
    }
}