﻿using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WizardOption : BaseOption
{
    public WizardOption(Rectangle sourceRect) :
        base(I18n.Option_Wizard(), sourceRect) { }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk)
            Game1.currentLocation.ShowConstructOptions("Wizard");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}