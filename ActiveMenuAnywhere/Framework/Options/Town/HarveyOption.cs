using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class HarveyOption : BaseOption
{
    public HarveyOption(Rectangle sourceRect) :
        base(I18n.Option_Harvey(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}