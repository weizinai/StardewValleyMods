﻿using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SandyOption : BaseOption
{
    public SandyOption(Rectangle sourceRect) :
        base(I18n.Option_Sandy(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault"))
            Utility.TryOpenShopMenu("Sandy", "Sandy");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}