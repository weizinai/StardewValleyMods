using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DesertTradeOption : BaseOption
{
    public DesertTradeOption()
        : base(I18n.UI_Option_DesertTrade(), TextureManager.Instance.DesertTexture, GetSourceRectangle(1), OptionId.DesertTrade) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("DesertTrade", null, true);
    }
}