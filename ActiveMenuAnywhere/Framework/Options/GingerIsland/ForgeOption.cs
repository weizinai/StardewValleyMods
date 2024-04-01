using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class ForgeOption : BaseOption
{
    public ForgeOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Forge())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("willyHours"))
            Game1.activeClickableMenu = new StardewValley.Menus.ForgeMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}