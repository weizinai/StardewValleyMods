using Common.UI;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace HelpWanted.Framework.Menu;

internal abstract class BaseQuestBoard : IClickableMenu
{
    protected readonly ClickableComponent AcceptQuestButton;
    protected string HoverTitle = "";
    protected string HoverText = "";


    protected int ShowingQuestID;
    protected Quest? ShowingQuest;
    protected readonly ModConfig Config;
    
    protected Rectangle BoardRect = new(78 * 4, 52 * 4, 184 * 4, 102 * 4);
    protected const int OptionIndex = -4200;

    protected BaseQuestBoard(ModConfig config) : base(0,0,0,0,true)
    {
        // 位置和大小逻辑
        width = 338 * 4;
        height = 198 * 4;
        var center = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
        xPositionOnScreen = (int)center.X;
        yPositionOnScreen = (int)center.Y;
        
        // 接受任务按钮逻辑
        var stringSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));
        AcceptQuestButton = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 128, yPositionOnScreen + height - 128,
            (int)stringSize.X + 24, (int)stringSize.Y + 24), "");
        
        // 关闭按钮逻辑
        upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen, 48, 48),
            Game1.mouseCursors, CommonImage.CloseButton, 4f);
        
        // 初始化
        Config = config;
    }
}