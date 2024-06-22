﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class CasinoOption : BaseOption
{
    public CasinoOption(Rectangle sourceRect) :
        base(I18n.Option_Casino(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            Utility.TryOpenShopMenu("Casino", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}