using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JojaShopOption : BaseOption
{
    public override bool IsEnable()
    {
        return !Game1.MasterPlayer.hasCompletedCommunityCenter();
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Joja", "Claire");
    }
}
