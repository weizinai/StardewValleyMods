using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SandyOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("ccVault");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Sandy", "Sandy");
    }
}
