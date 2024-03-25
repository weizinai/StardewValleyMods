using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class BobberMenu : BaseActiveMenu
{
    public BobberMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("spring_2_1"))
            Game1.activeClickableMenu = new ChooseFromIconsMenu("bobbers");
        else
            Game1.drawObjectDialogue("");
    }
}