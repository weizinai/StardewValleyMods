using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SophiaOption : BaseOption
{
    public SophiaOption(Rectangle sourceRect) 
        : base(I18n.UI_Option_Sophia(), sourceRect) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("FlashShifter.StardewValleyExpandedCP_SophiaLedger", "Sophia");
    }
}