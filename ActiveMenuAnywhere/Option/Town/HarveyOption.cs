using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HarveyOption : BaseOption
{
    public override void Apply()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}
