using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JoiOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160254");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJioShop", null, false);
    }
}
