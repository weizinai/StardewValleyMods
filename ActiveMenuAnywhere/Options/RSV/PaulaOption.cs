using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class PaulaOption : BaseOption
{
    public PaulaOption(Rectangle sourceRect) : base(I18n.Option_Paula(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.PaulaClinic", "ClinicChoices").Invoke(null, null);
    }
}