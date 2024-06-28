using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class ForgeOption : BaseOption
{
    public ForgeOption(Rectangle sourceRect) :
        base(I18n.Option_Forge(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("willyHours"))
            Game1.activeClickableMenu = new ForgeMenu();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}