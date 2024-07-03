using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.LazyMod.Framework.Helper;

internal static class TileHelper
{
    private static readonly Dictionary<int, List<Vector2>> TileCache = new();

    public static List<Vector2> GetTileGrid(int range)
    {
        if (TileCache.TryGetValue(range, out var cache)) return cache;

        var origin = Game1.player.Tile;
        var grid = new List<Vector2>((range * 2 + 1) * (range * 2 + 1));
        for (var x = -range; x <= range; x++)
        {
            for (var y = -range; y <= range; y++)
            {
                grid.Add(new Vector2(origin.X + x, origin.Y + y));
            }
        }
        TileCache.Add(range, grid);
        return grid;
    }

    public static void ClearTileCache()
    {
        TileCache.Clear();
    }
}