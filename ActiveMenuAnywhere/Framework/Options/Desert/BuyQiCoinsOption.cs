using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class BuyQiCoinsOption : BaseOption
{
    public BuyQiCoinsOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : 
        base(bounds, texture, sourceRect,I18n.Option_BuyQiCoins())
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