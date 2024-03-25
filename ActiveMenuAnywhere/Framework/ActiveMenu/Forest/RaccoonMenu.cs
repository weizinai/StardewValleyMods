using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class RaccoonMenu: BaseActiveMenu
{
    private readonly IModHelper helper;
    
    public RaccoonMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        var isUnlocked = helper.Reflection.GetField<NetBool>(new Raccoon(), "mrs_raccoon").GetValue().Value;
        if (isUnlocked)
            Utility.TryOpenShopMenu("Raccoon", "Raccoon");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}