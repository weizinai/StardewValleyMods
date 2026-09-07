using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.PiCore;

public static class PositionHelper
{
    private const int TileSize = 64;

    public static Vector2 GetAbsolutePositionFromScreenPosition(Vector2 screenPosition)
    {
        return screenPosition + GetViewportPosition();
    }

    public static Vector2 GetAbsolutePositionFromTilePosition(Vector2 tilePosition, bool center = false)
    {
        return tilePosition * TileSize + (center ? new Vector2(TileSize / 2f) : Vector2.Zero);
    }

    public static Vector2 GetScreenPositionFromAbsolutePosition(Vector2 absolutePosition)
    {
        return absolutePosition - GetViewportPosition();
    }

    public static Vector2 GetScreenPositionFromTilePosition(Vector2 tilePosition, bool center = false)
    {
        return tilePosition * TileSize - GetViewportPosition() + (center ? new Vector2(TileSize / 2f) : Vector2.Zero);
    }

    public static Vector2 GetTilePositionFromAbsolutePosition(Vector2 absolutePosition)
    {
        return new Vector2((int)(absolutePosition.X / TileSize), (int)(absolutePosition.Y / TileSize));
    }

    public static Vector2 GetTilePositionFromScreenPosition(Vector2 screenPosition)
    {
        var viewport = GetViewportPosition();

        return new Vector2((int)((screenPosition.X + viewport.X) / TileSize), (int)((screenPosition.Y + viewport.Y) / TileSize));
    }

    public static Vector2 GetTilePositionFromMousePosition()
    {
        return GetTilePositionFromScreenPosition(new Vector2(Game1.getOldMouseX(false), Game1.getOldMouseY(false)));
    }

    // 视口会随玩家移动而滚动，每次换算都必须读取 Game1.viewport 的当前偏移；
    // 不能像旧实现那样在类型加载时只采样一次存成 static readonly，否则玩家一动屏幕↔世界换算就会漂移。
    private static Vector2 GetViewportPosition()
    {
        return new Vector2(Game1.viewport.X, Game1.viewport.Y);
    }
}
