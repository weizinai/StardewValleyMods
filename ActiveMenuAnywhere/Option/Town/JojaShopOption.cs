using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JojaShopOption : BaseOption
{
    public JojaShopOption(Rectangle sourceRect) :
        base(I18n.UI_Option_JojaShop(), sourceRect) { }

    public override bool IsEnable()
    {
        return !Game1.MasterPlayer.hasCompletedCommunityCenter();
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Joja", "Claire");
    }
}