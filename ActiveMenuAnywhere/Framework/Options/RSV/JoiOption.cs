using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

internal class JoiOption : BaseOption
{
    public JoiOption(Rectangle sourceRect) : base(I18n.Option_Joi(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160254"))
            Utility.TryOpenShopMenu("RSVJioShop", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}