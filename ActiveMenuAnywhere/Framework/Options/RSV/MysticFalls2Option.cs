using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options.RSV;

public class MysticFalls2Option : BaseOption
{
    public MysticFalls2Option(Rectangle sourceRect) : base("MysticFalls2", sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.eventsSeen.Contains("75160187"))
            Utility.TryOpenShopMenu("RSVMysticFalls2", null, false);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}