using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Forest;

public class WizardActiveMenu : BaseActiveMenu
{
    public WizardActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk)
            Game1.currentLocation.ShowConstructOptions("Wizard");
        else
            Game1.drawObjectDialogue("你现在还不能建筑魔法建筑");
    }
}