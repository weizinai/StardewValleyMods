using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class JoiOption : BaseOption
{
    public JoiOption(Rectangle sourceRect) : base("Joi", sourceRect)
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