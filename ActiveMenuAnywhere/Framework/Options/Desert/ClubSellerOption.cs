using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class ClubSellerOption : BaseOption
{
    public ClubSellerOption(Rectangle sourceRect) :
        base(I18n.Option_ClubSeller(), sourceRect)
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