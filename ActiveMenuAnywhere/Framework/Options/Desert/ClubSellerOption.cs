using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class ClubSellerOption : BaseOption
{
    public ClubSellerOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : 
        base(bounds, texture, sourceRect, I18n.Option_ClubSeller())
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            ClubSeller();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }

    private void ClubSeller()
    {
        var options = new List<Response>
        {
            new("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes")),
            new("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No"))
        };
        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller"),
            options.ToArray(), "ClubSeller");
    }
}