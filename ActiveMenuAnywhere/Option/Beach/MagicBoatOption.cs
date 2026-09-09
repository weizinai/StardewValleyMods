using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MagicBoatOption : BaseOption
{
    public override bool IsEnable()
    {
        return Utility.IsPassiveFestivalDay("NightMarket");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Festival_NightMarket_MagicBoat_Day" + Utility.GetDayOfPassiveFestival("NightMarket"), null, false);
    }
}
