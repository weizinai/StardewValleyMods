using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class WillyOption : BaseOption
{
    public WillyOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Willy())
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("spring_2_1"))
            Utility.TryOpenShopMenu("FishShop", "Willy");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}