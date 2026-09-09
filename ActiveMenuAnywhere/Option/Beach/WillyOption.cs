using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WillyOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("spring_2_1");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("FishShop", "Willy");
    }
}
