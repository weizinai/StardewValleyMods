using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

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