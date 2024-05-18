using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

internal class BobberOption : BaseOption
{
    public BobberOption(Rectangle sourceRect) :
        base(I18n.Option_Bobber(), sourceRect)
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