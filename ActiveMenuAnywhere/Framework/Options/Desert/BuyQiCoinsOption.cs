using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class BuyQiCoinsOption : BaseOption
{
    public BuyQiCoinsOption(Rectangle sourceRect) :
        base(I18n.Option_BuyQiCoins(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            BuyQiCoins();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void BuyQiCoins()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins"),
            location.createYesNoResponses(), "BuyQiCoins");
    }
}