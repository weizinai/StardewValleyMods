using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class JojaShopOption : BaseOption
{
    public JojaShopOption()
        : base(I18n.UI_Option_JojaShop(), TextureManager.Instance.TownTexture, GetSourceRectangle(6), OptionId.JojaShop) { }

    public override bool IsEnable()
    {
        return !Game1.MasterPlayer.hasCompletedCommunityCenter();
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Joja", "Claire");
    }
}