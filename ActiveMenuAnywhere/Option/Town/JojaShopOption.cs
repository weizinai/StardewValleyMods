using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JojaShopOption : BaseOption
{
    public JojaShopOption() : base(I18n.UI_Option_JojaShop(), GetSourceRectangle(6)) { }

    public override bool IsEnable()
    {
        return !Game1.MasterPlayer.hasCompletedCommunityCenter();
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Joja", "Claire");
    }
}