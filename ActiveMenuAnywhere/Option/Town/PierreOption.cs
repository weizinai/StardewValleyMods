using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class PierreOption : BaseOption
{
    public PierreOption(Rectangle sourceRect) :
        base(I18n.Option_Pierre(), sourceRect) { }

    public override void Apply()
    {
        var options = new List<Response>
        {
            new("SeedShop", I18n.PierreOption_SeedShop()),
            new("BuyBackpack", I18n.PierreOption_BuyBackpack()),
            new("Leave", I18n.BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterQuestionBehavior);
    }

    private void AfterQuestionBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "SeedShop":
                Utility.TryOpenShopMenu("SeedShop", "Pierre");
                break;
            case "BuyBackpack":
                Game1.currentLocation.performAction("BuyBackpack", Game1.player, new Location());
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}