using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class TravelerOption : BaseOption
{
    public TravelerOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_Traveler())
    {
    }

    public override void ReceiveLeftClick()
    {
        var shouldTravelingMerchantVisitToday = Game1.dayOfMonth % 7 % 5 == 0;
        if (shouldTravelingMerchantVisitToday)
            Utility.TryOpenShopMenu("Traveler", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}