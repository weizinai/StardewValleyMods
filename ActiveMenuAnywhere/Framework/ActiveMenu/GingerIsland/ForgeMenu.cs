using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class ForgeMenu: BaseActiveMenu
{
    public ForgeMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.locationsVisited.Contains("Caldera"))
            Game1.activeClickableMenu = new StardewValley.Menus.ForgeMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}