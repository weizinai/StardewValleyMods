using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

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