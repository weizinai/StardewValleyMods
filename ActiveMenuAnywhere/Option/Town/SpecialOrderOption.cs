using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SpecialOrderOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("15389722");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard();
    }
}
