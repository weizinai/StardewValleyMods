using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class DyeOption : BaseOption
{
    public DyeOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Dye())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("992559"))
            Game1.activeClickableMenu = new DyeMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}