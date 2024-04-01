﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class HatMouseOption : BaseOption
{
    public HatMouseOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_HatMouse())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.achievements.Count > 0)
            Utility.TryOpenShopMenu("HatMouse", null, true);
        else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_HatMouseStore_Abandoned"));
    }
}