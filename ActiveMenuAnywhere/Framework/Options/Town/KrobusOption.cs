using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class KrobusOption : BaseOption
{
    public KrobusOption(Rectangle sourceRect) :
        base(I18n.Option_Krobus(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.hasRustyKey)
            Utility.TryOpenShopMenu("ShadowShop", "Krobus");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}