﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class RaccoonOption : BaseOption
{
    private readonly IModHelper helper;

    public RaccoonOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_Raccoon(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.mailReceived.Contains("raccoonMovedIn"))
        {
            var options = new List<Response>();
            var day = Game1.netWorldState.Value.Date.TotalDays - Game1.netWorldState.Value.DaysPlayedWhenLastRaccoonBundleWasFinished;
            if (day >= 7)
                options.Add(new Response("RaccoonBundle", "RaccoonBundle"));
            var mrsRaccoon = Game1.RequireLocation<Forest>("Forest").getCharacterFromName("MrsRaccoon");
            if (mrsRaccoon != null)
                options.Add(new Response("MrsRaccoonShop", "MrsRaccoonShop"));
            options.Add(new Response("Leave", "Leave"));
            Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "RaccoonBundle":
                this.helper.Reflection.GetMethod(new Raccoon(), "_activateMrRaccoon").Invoke();
                break;
            case "MrsRaccoonShop":
                Utility.TryOpenShopMenu("Raccoon", "Raccoon");
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}