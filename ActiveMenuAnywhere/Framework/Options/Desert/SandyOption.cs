using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

public class SandyOption : BaseOption
{
    public SandyOption(Rectangle sourceRect) :
        base(I18n.Option_Sandy(), sourceRect)
    {
    }


    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault"))
            Utility.TryOpenShopMenu("Sandy", "Sandy");
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}