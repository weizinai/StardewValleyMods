﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

public class MonsterOption : BaseOption
{
    private readonly IModHelper helper;

    public MonsterOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_Monster(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("guildMember"))
            helper.Reflection.GetMethod(new AdventureGuild(), "showMonsterKillList").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}