using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class IslandResortOption : BaseOption
{
    public IslandResortOption()
        : base(I18n.UI_Option_IslandResort(), TextureManager.Instance.GingerIslandTexture, GetSourceRectangle(4), OptionId.IslandResort) { }

    public override bool IsEnable()
    {
        return Game1.RequireLocation<IslandSouth>("IslandSouth").resortOpenToday.Value;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ResortBar", null, true);
    }
}