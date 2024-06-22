﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class DesertTradeOption : BaseOption
{
    public DesertTradeOption(Rectangle sourceRect) :
        base(I18n.Option_DesertTrade(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault"))
            Utility.TryOpenShopMenu("DesertTrade", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}