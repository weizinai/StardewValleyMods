using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class NightMarketTraveler : BaseOption
{
    public NightMarketTraveler()
        : base(I18n.UI_Option_NightMarketTraveler(), TextureManager.Instance.BeachTexture, GetSourceRectangle(2), OptionId.NightMarketTraveler) { }

    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, false);
    }
}