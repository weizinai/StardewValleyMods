using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class QiSpecialOrderOption : BaseOption
{
    public override bool IsEnable()
    {
        return IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
    }
}
