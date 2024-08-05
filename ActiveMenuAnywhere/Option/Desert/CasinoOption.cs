using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class CasinoOption : BaseOption
{
    public CasinoOption() : base(I18n.UI_Option_Casino(), GetSourceRectangle(2)) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Casino", null, true);
    }
}