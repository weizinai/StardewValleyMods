using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TravelerOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.dayOfMonth % 7 % 5 == 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, true);
    }
}
