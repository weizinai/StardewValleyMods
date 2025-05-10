using System.Collections.Generic;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class ClubSellerOption : BaseOption
{
    public ClubSellerOption()
        : base(I18n.UI_Option_ClubSeller(), TextureManager.Instance.DesertTexture, GetSourceRectangle(5), OptionId.ClubSeller) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard;
    }

    public override void Apply()
    {
        var options = new List<Response>
        {
            new("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes")),
            new("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No"))
        };
        Game1.currentLocation.createQuestionDialogue(
            Game1.content.LoadString("Strings\\Locations:Club_ClubSeller"),
            options.ToArray(),
            "ClubSeller"
        );
    }
}