using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IceCreamStandOption : BaseOption
{
    public IceCreamStandOption(Rectangle sourceRect) :
        base(I18n.UI_Option_IceCreamStand(), sourceRect) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}