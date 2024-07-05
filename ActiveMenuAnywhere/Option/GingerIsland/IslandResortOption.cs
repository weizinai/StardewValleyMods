using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IslandResortOption : BaseOption
{
    public IslandResortOption(Rectangle sourceRect) :
        base(I18n.UI_Option_IslandResort(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandSouth>("IslandSouth").resortOpenToday.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ResortBar", null, true);
    }
}