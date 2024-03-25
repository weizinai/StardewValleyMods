using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class QiGemShopMenu: BaseActiveMenu
{
    public QiGemShopMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out var actualFoundWalnutsCount);
        if (isQiWalnutRoomDoorUnlocked)
            Utility.TryOpenShopMenu("QiGemShop", null, playOpenSound: true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}