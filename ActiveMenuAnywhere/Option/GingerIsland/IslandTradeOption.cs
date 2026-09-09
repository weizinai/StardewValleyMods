using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandTradeOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandNorth>("IslandNorth").traderActivated.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IslandTrade", null, true);
    }
}
