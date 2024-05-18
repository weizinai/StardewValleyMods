using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

internal class TailoringOption : BaseOption
{
    public TailoringOption(Rectangle sourceRect) :
        base(I18n.Option_Tailoring(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("992559"))
            Game1.activeClickableMenu = new TailoringMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}