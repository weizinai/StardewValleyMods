using ActiveMenuAnywhere.Framework.ActiveMenu;
using ActiveMenuAnywhere.Framework.ActiveMenu.Beach;
using ActiveMenuAnywhere.Framework.ActiveMenu.Desert;
using ActiveMenuAnywhere.Framework.ActiveMenu.Farm;
using ActiveMenuAnywhere.Framework.ActiveMenu.Forest;
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
                    new TVActiveMenu(GetBoundsRectangle(0), textures[MenuTabID.Farm], GetSourceRectangle(0), helper),
                    new ShippingBinActiveMenu(GetBoundsRectangle(1), textures[MenuTabID.Farm], GetSourceRectangle(1))
                });
                break;
            case MenuTabID.Town1:
                break;
            case MenuTabID.Town2:
                break;
            case MenuTabID.Mountain:
                options.AddRange(new BaseActiveMenu[]
                {
                    new RobinActiveMenu(GetBoundsRectangle(0), textures[MenuTabID.Mountain], GetSourceRectangle(0)),
                    new DwarfActiveMenu(GetBoundsRectangle(1), textures[MenuTabID.Mountain], GetSourceRectangle(1)),
                    new MonsterActiveMenu(GetBoundsRectangle(2), textures[MenuTabID.Mountain], GetSourceRectangle(2), helper),
                    new MarlonActiveMenu(GetBoundsRectangle(3), textures[MenuTabID.Mountain], GetSourceRectangle(3), helper),
                });
                break;
            case MenuTabID.Forest:
                options.AddRange(new BaseActiveMenu[]
                {
                    new MarnieActiveMenu(GetBoundsRectangle(0), textures[MenuTabID.Forest], GetSourceRectangle(0)),
                    new TravelerActiveMenu(GetBoundsRectangle(1), textures[MenuTabID.Forest], GetSourceRectangle(1)),
                    new HatMouseActiveMenu(GetBoundsRectangle(2), textures[MenuTabID.Forest], GetSourceRectangle(2)),
                    new WizardActiveMenu(GetBoundsRectangle(3), textures[MenuTabID.Forest], GetSourceRectangle(3)),
                });
                break;
            case MenuTabID.Beach:
                options.AddRange(new BaseActiveMenu[]
                {
                    new WillyActiveMenu(GetSourceRectangle(0), textures[MenuTabID.Beach], GetBoundsRectangle(0)),
                    new BobberActiveMenu(GetSourceRectangle(1), textures[MenuTabID.Beach], GetBoundsRectangle(1)),
                });
                break;
            case MenuTabID.Desert:
                options.AddRange(new BaseActiveMenu[]
                {
                    new SandyActiveMenu(GetBoundsRectangle(0), textures[MenuTabID.Desert], GetSourceRectangle(0)),
                    new DesertTradeActiveMenu(GetBoundsRectangle(1), textures[MenuTabID.Desert], GetSourceRectangle(1)),
                    new CasinoActiveMenu(GetBoundsRectangle(2), textures[MenuTabID.Desert], GetSourceRectangle(2)),
                    new FarmerFileActiveMenu(GetBoundsRectangle(3), textures[MenuTabID.Desert], GetSourceRectangle(3), helper),
                    new BuyQiCoinsActiveMenu(GetBoundsRectangle(4), textures[MenuTabID.Desert], GetSourceRectangle(4)),
                    new ClubSellerActiveMenu(GetBoundsRectangle(5), textures[MenuTabID.Desert], GetSourceRectangle(5)),
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

    private Rectangle GetBoundsRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(innerDrawPosition.x + i * 200, innerDrawPosition.y + j * 200, 200, 200);
    }

    private Rectangle GetSourceRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(i * 200, j * 200, 200, 200);
    }
}