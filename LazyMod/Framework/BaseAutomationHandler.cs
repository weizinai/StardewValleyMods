using Microsoft.Xna.Framework;
using StardewValley;
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

    public abstract bool IsEnable();

    public abstract void Apply();
    
    protected Vector2 GetTilePixelPosition(Vector2 tile, bool center = true)
    {
        return tile * Game1.tileSize + (center ? new Vector2(Game1.tileSize / 2f) : Vector2.Zero);
    }

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var tilePixelPosition = this.GetTilePixelPosition(tile, false);
        return new Rectangle((int)tilePixelPosition.X, (int)tilePixelPosition.Y, Game1.tileSize, Game1.tileSize);
    }
}