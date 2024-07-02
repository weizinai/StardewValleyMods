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

    public abstract bool IsEnable();

    public abstract void Apply();

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(tile);
        return new Rectangle((int)position.X, (int)position.Y, Game1.tileSize, Game1.tileSize);
    }
}