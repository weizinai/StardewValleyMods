using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

internal class IceCreamStandOption : BaseOption
{
    public IceCreamStandOption(Rectangle sourceRect) :
        base(I18n.Option_IceCreamStand(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}