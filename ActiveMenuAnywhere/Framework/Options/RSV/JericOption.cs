using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class JericOption : BaseOption
{
    public JericOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_Jeric())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}