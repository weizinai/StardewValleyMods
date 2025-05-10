using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class IanOption : BaseOption
{
    public IanOption()
        : base(I18n.UI_Option_Ian(), TextureManager.Instance.RSVTexture, GetSourceRectangle(2), OptionId.Ian) { }

    public override void Apply()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.IanShop", "IanCounterMenu").Invoke(null, null);
    }
}