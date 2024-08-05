using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WillyOption : BaseOption
{
    public WillyOption() : base(I18n.UI_Option_Willy(), TextureManager.Instance.BeachTexture, GetSourceRectangle(0), OptionId.Willy) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("spring_2_1");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("FishShop", "Willy");
    }
}