using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HarveyOption : BaseOption
{
    public HarveyOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Harvey(), sourceRect) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Hospital", "Harvey");
    }
}