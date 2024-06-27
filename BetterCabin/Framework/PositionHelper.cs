using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal static class PositionHelper
{
    private const int TileSize = 64;
    
    public static Vector2 GetTilePositionFromScreenPosition(Vector2 screenPosition)
    {
        return new Vector2((screenPosition.X + Game1.viewport.X) / TileSize, 
            (screenPosition.Y + Game1.viewport.Y) / TileSize);
    }
}