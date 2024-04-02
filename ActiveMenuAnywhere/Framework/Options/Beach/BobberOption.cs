using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class BobberOption : BaseOption
{
    public BobberOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_Bobber())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("spring_2_1"))
            Game1.activeClickableMenu = new ChooseFromIconsMenu("bobbers");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}