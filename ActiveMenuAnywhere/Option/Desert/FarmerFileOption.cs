using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class FarmerFileOption : BaseOption
{
    public FarmerFileOption(Rectangle sourceRect) :
        base(I18n.Option_FarmerFile(), sourceRect) { }

    public override void ReceiveLeftClick()
    {
        if (Game1.player.mailReceived.Contains("ccVault") && Game1.player.hasClubCard)
            Game1.currentLocation.farmerFile();
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}