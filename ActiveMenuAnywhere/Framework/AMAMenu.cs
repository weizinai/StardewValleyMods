using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework;

public class AMAMenu : IClickableMenu
{
    private readonly IModHelper helper;
    private readonly Dictionary<MenuTabID, Texture2D> textures;

    private const int InnerWidth = 600;
    private const int InnerHeight = 600;

    private (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);


    private MenuTabID currentMenuTabID;
    private ClickableComponent title;
    private readonly List<ClickableComponent> tabs = new();
    private readonly List<ClickableTextureComponent> options = new();

    private ActiveMenuManager activeMenuManager;

    public AMAMenu(MenuTabID menuTabID, IModHelper helper, Dictionary<MenuTabID, Texture2D> textures)
    {
        this.helper = helper;
        this.textures = textures;
        activeMenuManager = new ActiveMenuManager(helper);
        Init(menuTabID);
        ResetComponents();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var tabLabel in from tab in tabs where tab.containsPoint(x, y) select GetTabID(tab))
        {
            Game1.activeClickableMenu = new AMAMenu(tabLabel, helper, textures);
            break;
        }

        
        foreach (var option in options.Where(option => option.containsPoint(x, y)))
        {
            // Game1.exitActiveMenu();
            helper.Reflection.GetMethod(new TV(),"checkForAction").Invoke(Game1.player, false);
            break;
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
            var tabID = GetTabID(tab);
            DrawHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right,
                tabID == currentMenuTabID ? 0.7f : 1f);
        }

        // Draw options
        foreach (var option in options)
            option.draw(spriteBatch);

        // Draw Mouse
        drawMouse(spriteBatch);
    }

    private void Init(MenuTabID menuTabID)
    {
        width = InnerWidth + borderWidth * 2;
        height = InnerHeight + borderWidth * 2;
        xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

        currentMenuTabID = menuTabID;
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
                "Farm", MenuTabID.Farm.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height, tabSize.width - tabOffset.x, tabSize.height),
                "Town", MenuTabID.Town.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Mountain", MenuTabID.Mountain.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Forest", MenuTabID.Forest.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Beach", MenuTabID.Beach.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "GingerIsland", MenuTabID.GingerIsland.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "SVE", MenuTabID.SVE.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i, tabSize.width - tabOffset.x, tabSize.height),
                "RSV", MenuTabID.RSV.ToString()),
        });

        // Add options
        options.Clear();
        switch (currentMenuTabID)
        {
            case MenuTabID.Farm:
                options.AddRange(new[]
                {
                    new ClickableTextureComponent(new Rectangle(innerDrawPosition.x, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Farm],
                        new Rectangle(0, 0, 200, 200), 1f)
                });
                break;
            case MenuTabID.Town:
                break;
            case MenuTabID.Mountain:
                break;
            case MenuTabID.Forest:
                break;
            case MenuTabID.Beach:
                break;
            case MenuTabID.GingerIsland:
                break;
            case MenuTabID.SVE:
                break;
            case MenuTabID.RSV:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private MenuTabID GetTabID(ClickableComponent tab)
    {
        if (!Enum.TryParse(tab.label, out MenuTabID tabID))
            throw new InvalidOperationException($"Couldn't parse tab name '{tab.label}'.");
        return tabID;
    }
}