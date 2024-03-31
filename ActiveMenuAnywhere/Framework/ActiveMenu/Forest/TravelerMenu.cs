using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class TravelerMenu : BaseActiveMenu
{
    public TravelerMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
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