using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class SpecialOrderMenu : BaseActiveMenu
{
    public SpecialOrderMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture,
        sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("15389722"))
            Game1.activeClickableMenu = new SpecialOrdersBoard();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}