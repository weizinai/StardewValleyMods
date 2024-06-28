using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class LorenzoOption : BaseOption
{
    public LorenzoOption(Rectangle sourceRect) :
        base(I18n.Option_Lorenzo(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}