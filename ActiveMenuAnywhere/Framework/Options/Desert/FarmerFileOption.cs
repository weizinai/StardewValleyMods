using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class FarmerFileOption : BaseOption
{
    public FarmerFileOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_FarmerFile())
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            Game1.currentLocation.farmerFile();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}