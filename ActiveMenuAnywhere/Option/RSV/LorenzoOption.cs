using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class LorenzoOption : BaseOption
{
    public LorenzoOption() : base(I18n.UI_Option_Lorenzo(), GetSourceRectangle(4)) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}