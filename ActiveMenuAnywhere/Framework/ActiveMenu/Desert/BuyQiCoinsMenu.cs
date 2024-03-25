using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class BuyQiCoinsMenu : BaseActiveMenu
{
    public BuyQiCoinsMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault")  && Game1.player.hasClubCard)
            BuyQiCoins();
        else
            Game1.drawObjectDialogue("不好意思，你还不能进入赌场");
    }

    private void BuyQiCoins()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins"), 
            location.createYesNoResponses(), "BuyQiCoins");
    }
}