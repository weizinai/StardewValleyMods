using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SandyOption : BaseOption
{
    public SandyOption()
        : base(I18n.UI_Option_Sandy(), TextureManager.Instance.DesertTexture, GetSourceRectangle(0), OptionId.Sandy) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Sandy", "Sandy");
    }
}