using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IceCreamStandOption : BaseOption
{
    public override void Apply()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}
