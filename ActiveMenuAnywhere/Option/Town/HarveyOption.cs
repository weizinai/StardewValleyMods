using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HarveyOption : BaseOption
{
    public HarveyOption() : base(I18n.UI_Option_Harvey(), GetSourceRectangle(11)) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}