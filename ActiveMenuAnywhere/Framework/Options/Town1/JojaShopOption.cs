using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class JojaShopOption : BaseOption
{
    public JojaShopOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect,
        I18n.Option_JojaShop())
    {
    }

    public override void ReceiveLeftClick()
    {
        if (!Game1.MasterPlayer.hasCompletedCommunityCenter())
            Utility.TryOpenShopMenu("Joja", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}