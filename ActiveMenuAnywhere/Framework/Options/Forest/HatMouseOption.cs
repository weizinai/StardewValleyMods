using Microsoft.Xna.Framework;
using StardewValley;

namespace ActiveMenuAnywhere.Framework.Options;

internal class HatMouseOption : BaseOption
{
    public HatMouseOption(Rectangle sourceRect) :
        base(I18n.Option_HatMouse(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.achievements.Count > 0)
            Utility.TryOpenShopMenu("HatMouse", null, true);
        else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Forest_HatMouseStore_Abandoned"));
    }
}