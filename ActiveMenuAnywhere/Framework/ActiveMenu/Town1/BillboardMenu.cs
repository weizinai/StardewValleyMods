using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class BillboardMenu : BaseActiveMenu
{
    public BillboardMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect) : base(bounds, texture, sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var options = new List<Response>()
        {
            new("Calendar", "日历"),
            new("DailyQuest", "每日任务"),
            new("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
        };
        Game1.currentLocation.createQuestionDialogue("", options.ToArray(), AfterDialogueBehavior);
    }

    private void AfterDialogueBehavior(Farmer who, string whichAnswer)
    {
        switch (whichAnswer)
        {
            case "Calendar":
                Game1.activeClickableMenu=new Billboard();
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