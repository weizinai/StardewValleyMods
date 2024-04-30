## Utility中的实用方法

### 判断是否在玩家周围

``` csharp
// 瓦片坐标
public static bool tileWithinRadiusOfPlayer(int xTile, int yTile, int tileRadius, Farmer f)
{
    var point = new Point(xTile, yTile);
    var playerTile = f.Tile;
    if (Math.Abs(point.X - playerTile.X) <= tileRadius) return Math.Abs(point.Y - playerTile.Y) <= tileRadius;
    return false;
}
// 像素坐标
public static bool withinRadiusOfPlayer(int x, int y, int tileRadius, Farmer f)
{
    var point = new Point(x / 64, y / 64);
    var playerTile = f.Tile;
    if (Math.Abs(point.X - playerTile.X) <= tileRadius) return Math.Abs(point.Y - playerTile.Y) <= tileRadius;
    return false;
}
```

