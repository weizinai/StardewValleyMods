﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class MarnieOption : BaseOption
{
    public MarnieOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Marnie())
    {
    }

    public override void ReceiveLeftClick()
    {
        Game1.drawObjectDialogue(I18n.MarnieOption_Bug());
        var options = new List<Response>
        {
            new("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
            new("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
            new("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
        };
        if (Game1.player.mailReceived.Contains("MarniePetAdoption") || Game1.player.mailReceived.Contains("MarniePetRejectedAdoption"))
            options.Insert(2, new Response("Adopt", Game1.content.LoadString("Strings\\1_6_Strings:AdoptPets")));
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Supplies":
                Utility.TryOpenShopMenu("AnimalShop", "Marnie");
                break;
            case "Purchase":
                // Game1.player.forceCanMove();
                Game1.currentLocation.ShowAnimalShopMenu();
                break;
            case "Adopt":
                Utility.TryOpenShopMenu("PetAdoption", "Marnie");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                break;
        }
    }
}