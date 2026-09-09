using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JericOption : BaseOption
{
    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}
