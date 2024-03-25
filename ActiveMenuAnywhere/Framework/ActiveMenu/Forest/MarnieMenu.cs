using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class MarnieMenu : BaseActiveMenu
{
    public MarnieMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
            new("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
            new("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
        };
        if (Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
            options.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), "Marnie");
    }
}