using Microsoft.Xna.Framework;
using StardewValley;

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

    protected T? FindToolFromInventory<T>() where T : Tool
    {
        var player = Game1.player;
        if (player.CurrentTool is T tool)
            return tool;
        return player.Items.FirstOrDefault(item => item is T) as T;
    }

    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        // 获取该瓦片的中心像素坐标
        var toolPixelPosition = tile * Game1.tileSize + new Vector2(Game1.tileSize / 2f);
        tool.DoFunction(location, (int)toolPixelPosition.X, (int)toolPixelPosition.Y, 1, player);
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
        if (item.Stack <= 0)
            player.removeItemFromInventory(item);
    }
}