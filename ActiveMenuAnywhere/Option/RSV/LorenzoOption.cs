using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class LorenzoOption : BaseOption
{
    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}
