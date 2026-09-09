using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class ForgeOption : BaseOption
{
    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("willyHours");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new ForgeMenu();
    }
}
