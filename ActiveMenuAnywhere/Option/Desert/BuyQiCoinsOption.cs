using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class BuyQiCoinsOption : BaseOption
{
    public BuyQiCoinsOption(Rectangle sourceRect) :
        base(I18n.UI_Option_BuyQiCoins(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins"), location.createYesNoResponses(), "BuyQiCoins");
    }
}