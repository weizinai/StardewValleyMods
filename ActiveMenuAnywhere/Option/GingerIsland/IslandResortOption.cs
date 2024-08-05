using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandResortOption : BaseOption
{
    public IslandResortOption() : base(I18n.UI_Option_IslandResort(), GetSourceRectangle(4)) { }

    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandSouth>("IslandSouth").resortOpenToday.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ResortBar", null, true);
    }
}