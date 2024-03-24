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


    private MenuTabLabel currentMenuTabLabel;
    private ClickableComponent title;
    private readonly List<ClickableComponent> tabs = new();

    public AMAMenu(MenuTabLabel menuTabLabel)
    {
        Init(menuTabLabel);
        ResetComponents();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var tab in tabs)
        {
            if (tab.containsPoint(x, y))
            {
                var tabLabel = GetTabLabel(tab);
                Game1.activeClickableMenu = new AMAMenu(tabLabel);
            }
        }
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
        {
            var tabLabel = GetTabLabel(tab);
            DrawHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right, tabLabel == currentMenuTabLabel ? 0.7f : 1f);
        }
        
        // Draw Mouse
        drawMouse(spriteBatch);
    }

    private void Init(MenuTabLabel menuTabLabel)
    {
        width = InnerWidth + borderWidth * 2;
        height = InnerHeight + borderWidth * 2;
        xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

        currentMenuTabLabel = menuTabLabel;
    }

    private void ResetComponents()
    {
        // Add title
        const int titleOffsetY = -60;
        title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen + titleOffsetY, 0, 0),
            "ActiveMenuAnywhere");

        // Add tabs
        var tabOffset = (x: 4, y: 16);
        var tabSize = (width: 100, height: 48);
        var tabPosition = (x: xPositionOnScreen - tabSize.width, y: yPositionOnScreen + tabOffset.y);

        var i = 2;
        tabs.Clear();
        tabs.AddRange(new[]
        {
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y, tabSize.width - tabOffset.x, tabSize.height),
                "Farm", MenuTabLabel.Farm.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height, tabSize.width - tabOffset.x, tabSize.height),
                "Town", MenuTabLabel.Town.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Mountain", MenuTabLabel.Mountain.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Forest", MenuTabLabel.Forest.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Beach", MenuTabLabel.Beach.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "GingerIsland", MenuTabLabel.GingerIsland.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "SVE", MenuTabLabel.SVE.ToString()),
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i, tabSize.width - tabOffset.x, tabSize.height),
                "RSV", MenuTabLabel.RSV.ToString()),
        });
    }

    private MenuTabLabel GetTabLabel(ClickableComponent tab)
    {
        if (!Enum.TryParse(tab.label, out MenuTabLabel tabLabel))
            throw new InvalidOperationException($"Couldn't parse tab name '{tab.name}'.");
        return tabLabel;
    }
}