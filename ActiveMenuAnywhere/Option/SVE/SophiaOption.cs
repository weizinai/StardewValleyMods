using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SophiaOption : BaseOption
{
    public SophiaOption(Rectangle sourceRect) : base("Sophia", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("FlashShifter.StardewValleyExpandedCP_SophiaLedger", "Sophia");
    }
}