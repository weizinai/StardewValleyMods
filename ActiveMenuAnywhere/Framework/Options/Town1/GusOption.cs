using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class GusOption : BaseOption
{
    public GusOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,I18n.Option_Gus())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}