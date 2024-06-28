using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class BillboardOption : BaseOption
{
    public BillboardOption(Rectangle sourceRect) :
        base(I18n.Option_Billboard(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("Calendar", I18n.BillboardOption_Calendar()),
            new("DailyQuest", I18n.BillboardOption_DailyQuest()),
            new("Leave", I18n.BaseOption_Leave())
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), this.AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Calendar":
                Game1.activeClickableMenu = new Billboard();
                break;
            case "DailyQuest":
                Game1.activeClickableMenu = new Billboard(true);
                break;
            case "Leave":
                Game1.exitActiveMenu();
                Game1.player.forceCanMove();
                break;
        }
    }
}