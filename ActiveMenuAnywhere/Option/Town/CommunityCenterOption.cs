﻿using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class CommunityCenterOption : BaseOption
{
    private readonly List<string> keys;
    private readonly List<string> texts;

    public CommunityCenterOption()
        : base(I18n.UI_Option_CommunityCenter(), TextureManager.Instance.TownTexture, GetSourceRectangle(2), OptionId.CommunityCenter)
    {
        this.keys = new List<string> { "Pantry", "CraftsRoom", "FishTank", "BoilerRoom", "Vault", "Bulletin" };
        this.texts = new List<string>
        {
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Pantry"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_CraftsRoom"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_FishTank"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BoilerRoom"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_Vault"),
            Game1.content.LoadString("Strings\\Locations:CommunityCenter_AreaName_BulletinBoard")
        };
    }

    public override bool IsEnable()
    {
        return !Game1.player.mailReceived.Contains("JojaMember") && Game1.player.mailReceived.Contains("canReadJunimoText");
    }

    public override void Apply()
    {
        var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
        var options = new List<Response>();
        for (var i = 0; i < 6; i++)
        {
            if (communityCenter.shouldNoteAppearInArea(i))
            {
                options.Add(new Response(this.keys[i], this.texts[i]));
            }
        }

        options.Add(new Response("Leave", I18n.UI_BaseOption_Leave()));

        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        if (whichAnswer == "Leave")
        {
            Game1.exitActiveMenu();
            Game1.player.forceCanMove();
        }
        else
        {
            var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
            communityCenter.checkBundle(this.keys.IndexOf(whichAnswer));
        }
    }
}