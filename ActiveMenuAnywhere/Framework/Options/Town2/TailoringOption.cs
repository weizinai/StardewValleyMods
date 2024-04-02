using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class TailoringOption : BaseOption
{
    public TailoringOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_Tailoring())
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