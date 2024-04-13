using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace LazyMod.Framework;

public abstract class Automate
{
    public abstract void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item);

    protected IEnumerable<Vector2> GetTileGrid(Vector2 origin, int range)
    {
        for (var x = -range; x <= range; x++)
        for (var y = -range; y <= range; y++)
            yield return new Vector2(origin.X + x, origin.Y + y);
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
        tool.DoFunction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, 1, player);
    }

    protected bool StopAutomate(Farmer player, float stopAutomateStamina, ref bool hasAddMessage)
    {
        if (player.Stamina <= stopAutomateStamina)
        {
            if (!hasAddMessage)
                Game1.showRedMessage(I18n.MessageStamina());
            return true;
        }

        hasAddMessage = false;
        return false;
    }

    protected void ConsumeItem(Farmer player, Item item)
    {
        item.Stack--;
        if (item.Stack <= 0) player.removeItemFromInventory(item);
    }

    protected FarmAnimal? GetBestHarvestableFarmAnimal(GameLocation location, Tool tool, Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile, false);
        var animal = Utility.GetBestHarvestableFarmAnimal(location.Animals.Values, tool,
            new Rectangle((int)tilePixelPosition.X, (int)tilePixelPosition.Y, Game1.tileSize, Game1.tileSize));
        if (animal?.currentProduce.Value is null || animal.isBaby() || !animal.CanGetProduceWithTool(tool))
            return null;

        return animal;
    }

    /// <summary>
    ///     获取瓦片中心的像素坐标
    /// </summary>
    protected Vector2 GetTilePixelPosition(Vector2 tile, bool center = true)
    {
        return tile * Game1.tileSize + (center ? new Vector2(Game1.tileSize / 2f) : Vector2.Zero);
    }
}