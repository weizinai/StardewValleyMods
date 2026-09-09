using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class LolaOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160093");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
    }
}
