using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class PrizeTicketMenu: BaseActiveMenu
{
    public PrizeTicketMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Game1.activeClickableMenu = new StardewValley.Menus.PrizeTicketMenu();
    }
}