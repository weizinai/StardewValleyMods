using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class SpecialOrderOption : BaseOption
{
    public SpecialOrderOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_SpecialOrder())
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