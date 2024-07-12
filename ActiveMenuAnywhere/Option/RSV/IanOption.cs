using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IanOption : BaseOption
{
    public IanOption(Rectangle sourceRect)
        : base(I18n.UI_Option_Ian(), sourceRect) { }

    public override void Apply()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.IanShop", "IanCounterMenu").Invoke(null, null);
    }
}