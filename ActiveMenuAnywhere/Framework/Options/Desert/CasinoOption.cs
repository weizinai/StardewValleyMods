using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class CasinoOption : BaseOption
{
    public CasinoOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : 
        base(bounds, texture, sourceRect,I18n.Option_Casino())
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