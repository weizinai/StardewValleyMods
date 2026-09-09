using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HatMouseOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.achievements.Count > 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("HatMouse", null, true);
    }
}
