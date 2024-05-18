using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

internal class MagicBoatOption : BaseOption
{
    public MagicBoatOption(Rectangle sourceRect) : base("MagicBoat", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
            Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + Utility.GetDayOfPassiveFestival("NightMarket"), null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}