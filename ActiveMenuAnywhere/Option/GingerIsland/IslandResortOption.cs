using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandResortOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandSouth>("IslandSouth").resortOpenToday.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ResortBar", null, true);
    }
}
