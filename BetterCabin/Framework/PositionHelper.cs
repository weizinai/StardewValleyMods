using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal static class PositionHelper
{
    private const int TileSize = 64;
    
    public static Vector2 GetTilePositionFromScreenPosition(Vector2 screenPosition)
    {
        return new Vector2((int)((screenPosition.X + Game1.viewport.X) / TileSize), (int)((screenPosition.Y + Game1.viewport.Y) / TileSize));
    }

    public static Vector2 GetAbsolutePositionFromTilePosition(Vector2 tilePosition)
    {
        return new Vector2(tilePosition.X * TileSize, tilePosition.Y * TileSize);
    }

    public static Vector2 GetTilePositionFromMousePosition()
    {
        return GetTilePositionFromScreenPosition(new Vector2(Game1.getOldMouseX(false), Game1.getOldMouseY(false)));
    }
}