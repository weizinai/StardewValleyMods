using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public class BillboardOption : BaseOption
{
    public BillboardOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect, I18n.Option_Billboard())
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>
        {
            new("Calendar", I18n.BillboardMenu_Calendar()),
            new("DailyQuest", I18n.BillboardMenu_DailyQuest()),
            new("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
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
                break;
        }
    }
}