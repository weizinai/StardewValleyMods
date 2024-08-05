using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class WizardOption : BaseOption
{
    public WizardOption()
        : base(I18n.UI_Option_Wizard(), TextureManager.Instance.ForestTexture,GetSourceRectangle(3), OptionId.Wizard) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk;
    }

    public override void Apply()
    {
        Game1.currentLocation.ShowConstructOptions("Wizard");
    }
}