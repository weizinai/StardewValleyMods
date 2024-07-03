using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.Common;

internal static class PositionHelper
{
    private const int TileSize = 64;
    private static readonly Vector2 Viewport = new(Game1.viewport.X, Game1.viewport.Y);

    public static Vector2 GetAbsolutePositionFromScreenPosition(Vector2 screenPosition)
    {
        return screenPosition + Viewport;
    }

    public static Vector2 GetAbsolutePositionFromTilePosition(Vector2 tilePosition, bool center = false)
    {
        return tilePosition * TileSize + (center ? new Vector2(TileSize / 2f) : Vector2.Zero);
    }

    public static Vector2 GetScreenPositionFromAbsolutePosition(Vector2 absolutePosition)
    {
        return absolutePosition - Viewport;
    }

    public static Vector2 GetScreenPositionFromTilePosition(Vector2 tilePosition, bool center = false)
    {
        return tilePosition * TileSize - Viewport + (center ? new Vector2(TileSize / 2f) : Vector2.Zero);
    }

    public static Vector2 GetTilePositionFromAbsolutePosition(Vector2 absolutePosition)
    {
        return new Vector2((int)(absolutePosition.X / TileSize), (int)(absolutePosition.Y / TileSize));
    }

    public static Vector2 GetTilePositionFromScreenPosition(Vector2 screenPosition)
    {
        return new Vector2((int)((screenPosition.X + Game1.viewport.X) / TileSize), (int)((screenPosition.Y + Game1.viewport.Y) / TileSize));
    }

    public static Vector2 GetTilePositionFromMousePosition()
    {
        return GetTilePositionFromScreenPosition(new Vector2(Game1.getOldMouseX(false), Game1.getOldMouseY(false)));
    }
}