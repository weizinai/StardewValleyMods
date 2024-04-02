using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class KimpoiOption : BaseOption
{
    public KimpoiOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) :
        base(bounds, texture, sourceRect, I18n.Option_Kimpoi())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.MasterPlayer.eventsSeen.Contains("75160252"))
            Utility.TryOpenShopMenu("RSVKimpoiShop", "Kimpoi");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}