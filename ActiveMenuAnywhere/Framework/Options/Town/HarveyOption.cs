using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class HarveyOption : BaseOption
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