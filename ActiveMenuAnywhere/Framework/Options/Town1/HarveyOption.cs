using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class HarveyOption : BaseOption
{
    public HarveyOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Harvey())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}