﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MonsterOption : BaseOption
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
            this.helper.Reflection.GetMethod(new AdventureGuild(), "showMonsterKillList").Invoke();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}