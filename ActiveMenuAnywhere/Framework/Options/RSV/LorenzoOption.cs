using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class LorenzoOption : BaseOption
{
    public LorenzoOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_Lorenzo())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("RSVHeapsStore", "Lorenzo");
    }
}