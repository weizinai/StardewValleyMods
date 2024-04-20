using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class LorenzoOption : BaseOption
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