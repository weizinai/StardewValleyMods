using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IceCreamStandOption : BaseOption
{
    public IceCreamStandOption() : base(I18n.UI_Option_IceCreamStand(), GetSourceRectangle(14)) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}