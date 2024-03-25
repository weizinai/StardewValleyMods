using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class CasinoMenu : BaseActiveMenu
{
    public CasinoMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault")  && Game1.player.hasClubCard)
            Utility.TryOpenShopMenu("Casino", null, true);
        else
            Game1.drawObjectDialogue("不好意思，你还不能进入赌场");
    }
}