using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class NightMarketTraveler : BaseOption
{
    public NightMarketTraveler(Rectangle sourceRect) 
        : base(I18n.UI_Option_NightMarketTraveler(), sourceRect) { }

    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, false);
    }
}