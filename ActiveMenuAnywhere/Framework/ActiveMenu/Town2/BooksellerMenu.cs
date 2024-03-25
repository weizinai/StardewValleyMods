using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class BooksellerMenu: BaseActiveMenu
{
    public BooksellerMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth))
            Utility.TryOpenShopMenu("Bookseller", null, playOpenSound: true);
        else
            Game1.drawObjectDialogue("未解锁");
    }
}