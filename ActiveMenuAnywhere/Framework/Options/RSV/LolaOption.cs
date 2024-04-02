using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class LolaOption : BaseOption
{
    public LolaOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_Lola())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160093"))
            Utility.TryOpenShopMenu("RSVLolaShop", "Lola");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}