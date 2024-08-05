using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class GusOption : BaseOption
{
    public GusOption() : base(I18n.UI_Option_Gus(), GetSourceRectangle(5)) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}