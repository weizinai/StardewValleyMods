using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TravelerOption : BaseOption
{
    public TravelerOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Traveler(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.dayOfMonth % 7 % 5 == 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Traveler", null, true);
    }
}