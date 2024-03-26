using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class TailoringMenu : BaseActiveMenu
{
    public TailoringMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("992559"))
            Game1.activeClickableMenu = new StardewValley.Menus.TailoringMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}