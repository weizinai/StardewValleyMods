using System.Collections.Generic;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using xTile.Dimensions;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PierreOption : BaseOption
{
    public override void Apply()
    {
        var options = new List<Response>
        {
            new("SeedShop", I18n.UI_Option_Pierre_SeedShop()),
            new("BuyBackpack", I18n.UI_Option_Pierre_BuyBackpack()),
            new("Leave", I18n.UI_Common_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterQuestionBehavior);
    }

    private void AfterQuestionBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "SeedShop":
                {
                    Utility.TryOpenShopMenu("SeedShop", "Pierre");

                    break;
                }
            case "BuyBackpack":
                {
                    Game1.currentLocation.performAction("BuyBackpack", Game1.player, new Location());

                    break;
                }
            case "Leave":
                {
                    Game1.exitActiveMenu();
                    Game1.player.forceCanMove();

                    break;
                }
        }
    }
}
