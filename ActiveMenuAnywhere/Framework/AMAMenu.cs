using ActiveMenuAnywhere.Framework.ActiveMenu;
using ActiveMenuAnywhere.Framework.ActiveMenu.Beach;
using ActiveMenuAnywhere.Framework.ActiveMenu.Desert;
using ActiveMenuAnywhere.Framework.ActiveMenu.Farm;
using ActiveMenuAnywhere.Framework.ActiveMenu.Mountain;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework;

public class AMAMenu : IClickableMenu
{
    private readonly IModHelper helper;
    private readonly Dictionary<MenuTabID, Texture2D> textures;

    private const int InnerWidth = 600;
    private const int InnerHeight = 600;

    private readonly (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);


    private MenuTabID currentMenuTabID;
    private ClickableComponent title;
    private readonly List<ClickableComponent> tabs = new();
    private readonly List<BaseActiveMenu> options = new();

    public AMAMenu(MenuTabID menuTabID, IModHelper helper, Dictionary<MenuTabID, Texture2D> textures)
    {
        this.helper = helper;
        this.textures = textures;
        Init(menuTabID);
        ResetComponents();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var tabID in from tab in tabs where tab.containsPoint(x, y) select GetTabID(tab))
        {
            Game1.activeClickableMenu = new AMAMenu(tabID, helper, textures);
            break;
        }


        foreach (var option in options.Where(option => option.containsPoint(x, y)))
        {
            option.ReceiveLeftClick();
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
                "Town1", MenuTabID.Town1.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                "Town2", MenuTabID.Town2.ToString()),
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
                "Desert", MenuTabID.Desert.ToString()),
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
                options.AddRange(new BaseActiveMenu[]
                {
                    new TVActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Farm], new Rectangle(0, 0, 200, 200), helper),
                    new ShippingBinActiveMenu(new Rectangle(innerDrawPosition.x + 200, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Farm], new Rectangle(200, 0, 200, 200))
                });
                break;
            case MenuTabID.Town1:
                break;
            case MenuTabID.Town2:
                break;
            case MenuTabID.Mountain:
                options.AddRange(new BaseActiveMenu[]
                {
                    new RobinActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Mountain], new Rectangle(0, 0, 200, 200)),
                    new DwarfActiveMenu(new Rectangle(innerDrawPosition.x + 200, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Mountain], new Rectangle(200, 0, 200, 200)),
                    new MonsterActiveMenu(new Rectangle(innerDrawPosition.x + 400, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Mountain], new Rectangle(400, 0, 200, 200), helper),
                    new MarlonActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y + 200, 200, 200),
                        textures[MenuTabID.Mountain], new Rectangle(0, 200, 200, 200), helper),
                });
                break;
            case MenuTabID.Forest:
                break;
            case MenuTabID.Beach:
                options.AddRange(new BaseActiveMenu[]
                {
                    new WillyActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Beach], new Rectangle(0, 0, 200, 200)),
                    new BobberActiveMenu(new Rectangle(innerDrawPosition.x + 200, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Beach], new Rectangle(200, 0, 200, 200)),
                });
                break;
            case MenuTabID.Desert:
                options.AddRange(new BaseActiveMenu[]
                {
                    new SandyActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(0, 0, 200, 200)),
                    new DesertTradeActiveMenu(new Rectangle(innerDrawPosition.x + 200, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(200, 0, 200, 200)),
                    new CasinoActiveMenu(new Rectangle(innerDrawPosition.x + 400, innerDrawPosition.y, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(400, 0, 200, 200)),
                    new FarmerFileActiveMenu(new Rectangle(innerDrawPosition.x, innerDrawPosition.y + 200, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(0, 200, 200, 200), helper),
                    new BuyQiCoinsActiveMenu(new Rectangle(innerDrawPosition.x + 200, innerDrawPosition.y + 200, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(200, 200, 200, 200)),
                    new ClubSellerActiveMenu(new Rectangle(innerDrawPosition.x + 400, innerDrawPosition.y + 200, 200, 200),
                        textures[MenuTabID.Desert], new Rectangle(400, 200, 200, 200)),
                });
                break;
            case MenuTabID.GingerIsland:
                break;
            case MenuTabID.SVE:
                Game1.drawObjectDialogue("不好意思，该功能还未完成");
                break;
            case MenuTabID.RSV:
                Game1.drawObjectDialogue("不好意思，该功能还未完成");
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