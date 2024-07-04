using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WizardOption : BaseOption
{
    public WizardOption(Rectangle sourceRect) :
        base(I18n.Option_Wizard(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk;
    }

    public override void Apply()
    {
        Game1.currentLocation.ShowConstructOptions("Wizard");
    }
}