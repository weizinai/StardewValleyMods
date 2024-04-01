using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class WizardOption : BaseOption
{
    public WizardOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Wizard())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk)
            Game1.currentLocation.ShowConstructOptions("Wizard");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}