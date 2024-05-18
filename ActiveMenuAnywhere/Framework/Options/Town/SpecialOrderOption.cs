using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

internal class SpecialOrderOption : BaseOption
{
    public SpecialOrderOption(Rectangle sourceRect) :
        base(I18n.Option_SpecialOrder(), sourceRect)
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