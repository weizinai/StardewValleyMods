using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class BillboardOption : BaseOption
{
    public BillboardOption() : base(I18n.UI_Option_Billboard(), GetSourceRectangle(0)) { }

    public override void Apply()
    {
        var options = new List<Response>
        {
            new("Calendar", I18n.UI_BillboardOption_Calendar()),
            new("DailyQuest", I18n.UI_BillboardOption_DailyQuest()),
            new("Leave", I18n.UI_BaseOption_Leave())
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