using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework;

public class AMAMenu : IClickableMenu
{
    private const int InnerWidth = 600;
    private const int InnerHeight = 600;
    private (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);


    private MenuTab currentMenuTab;
    private ClickableComponent title;
    private readonly List<ClickableComponent> tabs = new();

    public AMAMenu(MenuTab menuTab)
    {
        Init(menuTab);
        ResetComponents();
    }


    public override void draw(SpriteBatch spriteBatch)
    {
        // Draw shadow
        DrawHelper.DrawShadow();

        // Draw background
        drawTextureBox(spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Draw title
        DrawHelper.DrawTitle(title.bounds.X, title.bounds.Y, title.name, Align.Center);

        // Draw tabs
        foreach (var tab in tabs)
            DrawHelper.DrawTab(tab.bounds.X, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right, 1f);
    }

    private void Init(MenuTab menuTab)
    {
        width = InnerWidth + borderWidth * 2;
        height = InnerHeight + borderWidth * 2;
        xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

        currentMenuTab = menuTab;
    }

    private void ResetComponents()
    {
        // Add title
        const int titleOffsetY = -60;
        title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen + titleOffsetY, 0, 0),
            "ActiveMenuAnywhere");

        // Add tabs
        var tabOffset = (x: 0, y: 0);
        var tabSize = (width: 100, height: 60);
        var tabPosition = (x: xPositionOnScreen - tabSize.width, y: yPositionOnScreen + tabOffset.y);

        var i = 2;
        tabs.Clear();
        tabs.AddRange(new[]
        {
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y, tabSize.width, tabSize.height),
                "Farm", MenuTab.Farm.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height, tabSize.width, tabSize.height),
                "Town", MenuTab.Town.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width, tabSize.height),
                "Mountain", MenuTab.Mountain.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width, tabSize.height),
                "Forest", MenuTab.Forest.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width, tabSize.height),
                "Beach", MenuTab.Beach.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width, tabSize.height),
                "GingerIsland", MenuTab.GingerIsland.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width, tabSize.height),
                "SVE", MenuTab.SVE.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i, tabSize.width, tabSize.height),
                "RSV", MenuTab.RSV.ToString()),
        });
    }
}