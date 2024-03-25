using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class QiCatMenu: BaseActiveMenu
{
    private IModHelper helper;
    
    public QiCatMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out var actualFoundWalnutsCount);
        if (isQiWalnutRoomDoorUnlocked)
            helper.Reflection.GetMethod(new GameLocation(), "ShowQiCat").Invoke();
        else
            Game1.drawObjectDialogue("你还没有解锁核桃房");
    }
}