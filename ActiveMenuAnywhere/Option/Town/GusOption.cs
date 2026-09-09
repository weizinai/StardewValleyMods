using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class GusOption : BaseOption
{
    public override void Apply()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}
