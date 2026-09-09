using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Helper;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class IanOption : BaseOption
{
    public override void Apply()
    {
        RSVReflection.GetRSVPrivateStaticMethod("RidgesideVillage.IanShop", "IanCounterMenu").Invoke(null, null);
    }
}
