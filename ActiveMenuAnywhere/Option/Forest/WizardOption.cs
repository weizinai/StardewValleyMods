using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WizardOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk;
    }

    public override void Apply()
    {
        Game1.currentLocation.ShowConstructOptions("Wizard");
    }
}
