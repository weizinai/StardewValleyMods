using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class AbandonedJojaMartMenu: BaseActiveMenu
{
    public AbandonedJojaMartMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var abandonedJojaMart = Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart");
        if (abandonedJojaMart != null)
            abandonedJojaMart.checkBundle();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}