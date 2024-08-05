using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JericOption : BaseOption
{
    public JericOption() : base(I18n.UI_Option_Jeric(), GetSourceRectangle(5)) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}