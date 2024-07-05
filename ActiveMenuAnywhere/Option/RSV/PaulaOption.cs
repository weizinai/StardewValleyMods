using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PaulaOption : BaseOption
{
    public PaulaOption(Rectangle sourceRect) 
        : base(I18n.UI_Option_Paula(), sourceRect) { }

    public override void Apply()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.PaulaClinic", "ClinicChoices").Invoke(null, null);
    }
}