using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class IceCreamStandOption : BaseOption
{
    public IceCreamStandOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_IceCreamStand())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("IceCreamStand", null, true);
    }
}