using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class IanOption : BaseOption
{
    public IanOption(Rectangle sourceRect) : base(I18n.Option_Ian(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.IanShop", "IanCounterMenu").Invoke(null, null);
    }
}