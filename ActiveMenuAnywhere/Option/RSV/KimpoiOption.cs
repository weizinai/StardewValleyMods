using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class KimpoiOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("75160252");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
    }
}
