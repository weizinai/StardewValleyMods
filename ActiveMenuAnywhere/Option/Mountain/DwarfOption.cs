using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class DwarfOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.canUnderstandDwarves;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Dwarf", "Dwarf");
    }
}
