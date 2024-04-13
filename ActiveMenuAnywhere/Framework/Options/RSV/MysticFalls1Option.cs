﻿using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class MysticFalls1Option : BaseOption
{
    public MysticFalls1Option(Rectangle sourceRect) : base(I18n.Option_MysticFall1(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160187"))
            Utility.TryOpenShopMenu("RSVMysticFalls1", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}