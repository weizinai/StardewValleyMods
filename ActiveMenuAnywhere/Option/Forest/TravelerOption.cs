using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class TravelerOption : BaseOption
{
    public TravelerOption()
        : base(I18n.UI_Option_Traveler(), TextureManager.Instance.ForestTexture, GetSourceRectangle(1), OptionId.Traveler) { }

    public override bool IsEnable()
    {
        return Game1.dayOfMonth % 7 % 5 == 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, true);
    }
}