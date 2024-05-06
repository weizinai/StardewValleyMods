using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public abstract class Automate
{
    protected readonly Dictionary<int, List<Vector2>> TileCache = new();

    public abstract void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item);

    protected List<Vector2> GetTileGrid(Farmer player, int range)
    {
        if (TileCache.TryGetValue(range, out var cache))
            return cache;

        var origin = player.Tile;
        var grid = new List<Vector2>();
        for (var x = -range; x <= range; x++)
            for (var y = -range; y <= range; y++)
                grid.Add(new Vector2(origin.X + x, origin.Y + y));
        TileCache.Add(range, grid);
        return grid;
    }

    protected T? FindToolFromInventory<T>(bool findScythe = false) where T : Tool
    {
        var player = Game1.player;
        if (player.CurrentTool is T tool)
        {
            if (findScythe && tool is MeleeWeapon scythe && scythe.isScythe())
                return tool;
            return tool;
        }

        foreach (var item in player.Items)
            if (findScythe && item is MeleeWeapon scythe && scythe.isScythe())
                return scythe as T;

        return player.Items.FirstOrDefault(item => item is T) as T;
    }

    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile);
        tool.swingTicker++;
        tool.DoFunction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, 1, player);
    }

    protected void ConsumeItem(Farmer player, Item item)
    {
        item.Stack--;
        if (item.Stack <= 0) player.removeItemFromInventory(item);
    }


    protected Vector2 GetTilePixelPosition(Vector2 tile, bool center = true)
    {
        return tile * Game1.tileSize + (center ? new Vector2(Game1.tileSize / 2f) : Vector2.Zero);
    }

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile, false);
        return new Rectangle((int)tilePixelPosition.X, (int)tilePixelPosition.Y, Game1.tileSize, Game1.tileSize);
    }

    protected void CheckTileAction(GameLocation location, Farmer player, Vector2 tile)
    {
        location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, player);
    }

    protected void HarvestMachine(Farmer player, SObject? machine)
    {
        if (machine is null) return;

        var heldObject = machine.heldObject.Value;
        if (machine.readyForHarvest.Value && heldObject is not null)
        {
            if (player.freeSpotsInInventory() == 0 && !player.Items.ContainsId(heldObject.ItemId)) return;
            machine.checkForAction(player);
        }
    }
}