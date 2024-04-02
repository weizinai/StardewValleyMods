using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class VolcanoShopOption : BaseOption
{
    public VolcanoShopOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_VolcanoShop())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("willyHours") && Game1.player.canUnderstandDwarves)
            Utility.TryOpenShopMenu("VolcanoShop", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}