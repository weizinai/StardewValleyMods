using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DecorationBoatOption : BaseOption
{
    public DecorationBoatOption()
        : base(I18n.UI_Option_DecorationBoat(), TextureManager.Instance.BeachTexture, GetSourceRectangle(3), OptionId.DecorationBoat) { }

    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", null, false);
    }
}