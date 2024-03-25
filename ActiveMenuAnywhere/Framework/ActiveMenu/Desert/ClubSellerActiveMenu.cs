﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Desert;

public class ClubSellerActiveMenu : BaseActiveMenu
{
    public ClubSellerActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            ClubSeller();
        else
            Game1.drawObjectDialogue("不好意思，你还不能进入赌场");
    }

    private void ClubSeller()
    {
        var location = Game1.currentLocation;
        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller"), new Response[2]
        {
            new Response("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes")),
            new Response("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No"))
        }, "ClubSeller");
    }
}