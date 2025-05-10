using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class CasinoOption : BaseOption
{
    public CasinoOption()
        : base(I18n.UI_Option_Casino(), TextureManager.Instance.DesertTexture, GetSourceRectangle(2), OptionId.Casino) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Casino", null, true);
    }
}