using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class VolcanoShopOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("willyHours") && Game1.player.canUnderstandDwarves;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("VolcanoShop", null, true);
    }
}
