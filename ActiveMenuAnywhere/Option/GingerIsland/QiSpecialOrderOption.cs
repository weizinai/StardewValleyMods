﻿using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class QiSpecialOrderOption : BaseOption
{
    public QiSpecialOrderOption(Rectangle sourceRect) :
        base(I18n.Option_QiSpecialOrder(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}