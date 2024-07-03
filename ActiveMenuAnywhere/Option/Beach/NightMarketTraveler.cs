using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class NightMarketTraveler : BaseOption
{
    public NightMarketTraveler(Rectangle sourceRect) : base("NightMarketTraveler", sourceRect) { }

    public override void ReceiveLeftClick()
    {
        if (Utility.IsPassiveFestivalDay("NightMarket"))
            Utility.TryOpenShopMenu("Traveler", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}