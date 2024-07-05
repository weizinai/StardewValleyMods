using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DecorationBoatOption : BaseOption
{
    public DecorationBoatOption(Rectangle sourceRect) 
        : base(I18n.UI_Option_DecorationBoat(), sourceRect) { }

    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Festival_NightMarket_DecorationBoat", null, false);
    }
}