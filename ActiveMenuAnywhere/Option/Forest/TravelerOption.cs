using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TravelerOption : BaseOption
{
    public TravelerOption() : base(I18n.UI_Option_Traveler(), GetSourceRectangle(1)) { }

    public override bool IsEnable()
    {
        return Game1.dayOfMonth % 7 % 5 == 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, true);
    }
}