using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class PierreOption : BaseOption
{
    public PierreOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Pierre())
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("SeedShop", "Pierre");
    }
}