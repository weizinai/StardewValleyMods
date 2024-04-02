using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

public class AbandonedJojaMartOption : BaseOption
{
    public AbandonedJojaMartOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_AbandonedJojaMart())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible"))
        {
            var abandonedJojaMart = Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart");
            abandonedJojaMart.checkBundle();
        }
        else
        {
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
        }
    }
}