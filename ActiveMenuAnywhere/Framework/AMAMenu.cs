using ActiveMenuAnywhere.Framework.ActiveMenu;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using DyeMenu = ActiveMenuAnywhere.Framework.ActiveMenu.DyeMenu;
using ForgeMenu = ActiveMenuAnywhere.Framework.ActiveMenu.ForgeMenu;
using PrizeTicketMenu = ActiveMenuAnywhere.Framework.ActiveMenu.PrizeTicketMenu;
using TailoringMenu = ActiveMenuAnywhere.Framework.ActiveMenu.TailoringMenu;

namespace ActiveMenuAnywhere.Framework;

public class AMAMenu : IClickableMenu
{
    private const int InnerWidth = 600;
    private const int InnerHeight = 600;
    private readonly IModHelper helper;

    private readonly (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);

    private readonly List<BaseActiveMenu> options = new();
    private readonly List<ClickableComponent> tabs = new();
    private readonly Dictionary<MenuTabID, Texture2D> textures;


    private MenuTabID currentMenuTabID;
    private ClickableComponent? title;

    public AMAMenu(MenuTabID menuTabID, IModHelper helper, Dictionary<MenuTabID, Texture2D> textures)
    {
        this.helper = helper;
        this.textures = textures;
        Init(menuTabID);
        ResetComponents();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // tab
        var tab = tabs.FirstOrDefault(tab => tab.containsPoint(x, y));
        if (tab != null) Game1.activeClickableMenu = new AMAMenu(GetTabID(tab), helper, textures);

        // option
        options.FirstOrDefault(option => option.containsPoint(x, y))?.ReceiveLeftClick();
    }

    public override void performHoverAction(int x, int y)
    {
        options.ForEach(option => option.scale = option.containsPoint(x, y) ? 0.9f : 1f);
    }

    public override void draw(SpriteBatch spriteBatch)
    {
        // Draw shadow
        DrawHelper.DrawShadow();

        // Draw background
        drawTextureBox(spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Draw title
        if (title != null)
            DrawHelper.DrawTitle(title.bounds.X, title.bounds.Y, title.name, Align.Center);

        // Draw tabs
        tabs.ForEach(tab => DrawHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.name, Align.Right,
            GetTabID(tab) == currentMenuTabID ? 0.7f : 1f));

        // Draw options
        options.ForEach(option => option.draw(spriteBatch));

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
        AddTab();

        // Add options
        AddOption();
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

    private void AddTab()
    {
        var tabOffset = (x: 4, y: 16);
        var tabSize = (width: 100, height: 48);
        var tabPosition = (x: xPositionOnScreen - tabSize.width, y: yPositionOnScreen + tabOffset.y);

        var i = 2;
        tabs.Clear();
        tabs.AddRange(new[]
        {
            new ClickableComponent(new Rectangle(tabPosition.x, tabPosition.y, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Farm(), MenuTabID.Farm.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Town1(), MenuTabID.Town1.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Town2(), MenuTabID.Town2.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Mountain(), MenuTabID.Mountain.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Forest(), MenuTabID.Forest.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Beach(), MenuTabID.Beach.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_Desert(), MenuTabID.Desert.ToString()),
            new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_GingerIsland(), MenuTabID.GingerIsland.ToString())
        });
        if (helper.ModRegistry.Get("FlashShifter.SVECode") != null)
            tabs.Add(new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i++, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_SVE(), MenuTabID.SVE.ToString()));
        if (helper.ModRegistry.Get("Rafseazz.RidgesideVillage") != null)
            tabs.Add(new ClickableComponent(
                new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * i, tabSize.width - tabOffset.x, tabSize.height),
                I18n.Tab_RSV(), MenuTabID.RSV.ToString()));
    }

    private void AddOption()
    {
        options.Clear();
        switch (currentMenuTabID)
        {
            case MenuTabID.Farm:
                options.AddRange(new BaseActiveMenu[]
                {
                    new TVMenu(GetBoundsRectangle(0), textures[MenuTabID.Farm], GetSourceRectangle(0), helper),
                    new ShippingBinMenu(GetBoundsRectangle(1), textures[MenuTabID.Farm], GetSourceRectangle(1), helper)
                });
                break;
            case MenuTabID.Town1:
                options.AddRange(new BaseActiveMenu[]
                {
                    new BillboardMenu(GetBoundsRectangle(0), textures[MenuTabID.Town1], GetSourceRectangle(0)),
                    new SpecialOrderMenu(GetBoundsRectangle(1), textures[MenuTabID.Town1], GetSourceRectangle(1)),
                    new CommunityCenterMenu(GetBoundsRectangle(2), textures[MenuTabID.Town1], GetSourceRectangle(2)),
                    new PierreMenu(GetBoundsRectangle(3), textures[MenuTabID.Town1], GetSourceRectangle(3)),
                    new ClintMenu(GetBoundsRectangle(4), textures[MenuTabID.Town1], GetSourceRectangle(4)),
                    new GusMenu(GetBoundsRectangle(5), textures[MenuTabID.Town1], GetSourceRectangle(5)),
                    new HarveyMenu(GetBoundsRectangle(6), textures[MenuTabID.Town1], GetSourceRectangle(6)),
                    new JojaShopMenu(GetBoundsRectangle(7), textures[MenuTabID.Town1], GetSourceRectangle(7))
                });
                break;
            case MenuTabID.Town2:
                options.AddRange(new BaseActiveMenu[]
                {
                    new IceCreamStandMenu(GetBoundsRectangle(0), textures[MenuTabID.Town2], GetSourceRectangle(0)),
                    new PrizeTicketMenu(GetBoundsRectangle(1), textures[MenuTabID.Town2], GetSourceRectangle(1)),
                    new BooksellerMenu(GetBoundsRectangle(2), textures[MenuTabID.Town2], GetSourceRectangle(2)),
                    new DyeMenu(GetBoundsRectangle(3), textures[MenuTabID.Town2], GetSourceRectangle(3)),
                    new TailoringMenu(GetBoundsRectangle(4), textures[MenuTabID.Town2], GetSourceRectangle(4)),
                    new AbandonedJojaMartMenu(GetBoundsRectangle(5), textures[MenuTabID.Town2], GetSourceRectangle(5)),
                    new KrobusMenu(GetBoundsRectangle(6), textures[MenuTabID.Town2], GetSourceRectangle(6)),
                    new StatueMenu(GetBoundsRectangle(7), textures[MenuTabID.Town2], GetSourceRectangle(7))
                });
                break;
            case MenuTabID.Mountain:
                options.AddRange(new BaseActiveMenu[]
                {
                    new RobinMenu(GetBoundsRectangle(0), textures[MenuTabID.Mountain], GetSourceRectangle(0)),
                    new DwarfMenu(GetBoundsRectangle(1), textures[MenuTabID.Mountain], GetSourceRectangle(1)),
                    new MonsterMenu(GetBoundsRectangle(2), textures[MenuTabID.Mountain], GetSourceRectangle(2), helper),
                    new MarlonMenu(GetBoundsRectangle(3), textures[MenuTabID.Mountain], GetSourceRectangle(3))
                });
                break;
            case MenuTabID.Forest:
                options.AddRange(new BaseActiveMenu[]
                {
                    new MarnieMenu(GetBoundsRectangle(0), textures[MenuTabID.Forest], GetSourceRectangle(0)),
                    new TravelerMenu(GetBoundsRectangle(1), textures[MenuTabID.Forest], GetSourceRectangle(1)),
                    new HatMouseMenu(GetBoundsRectangle(2), textures[MenuTabID.Forest], GetSourceRectangle(2)),
                    new WizardMenu(GetBoundsRectangle(3), textures[MenuTabID.Forest], GetSourceRectangle(3)),
                    new RaccoonMenu(GetBoundsRectangle(4), textures[MenuTabID.Forest], GetSourceRectangle(4), helper)
                });
                break;
            case MenuTabID.Beach:
                options.AddRange(new BaseActiveMenu[]
                {
                    new WillyMenu(GetBoundsRectangle(0), textures[MenuTabID.Beach], GetSourceRectangle(0)),
                    new BobberMenu(GetBoundsRectangle(1), textures[MenuTabID.Beach], GetSourceRectangle(1))
                });
                break;
            case MenuTabID.Desert:
                options.AddRange(new BaseActiveMenu[]
                {
                    new SandyMenu(GetBoundsRectangle(0), textures[MenuTabID.Desert], GetSourceRectangle(0)),
                    new DesertTradeMenu(GetBoundsRectangle(1), textures[MenuTabID.Desert], GetSourceRectangle(1)),
                    new CasinoMenu(GetBoundsRectangle(2), textures[MenuTabID.Desert], GetSourceRectangle(2)),
                    new FarmerFileMenu(GetBoundsRectangle(3), textures[MenuTabID.Desert], GetSourceRectangle(3), helper),
                    new BuyQiCoinsMenu(GetBoundsRectangle(4), textures[MenuTabID.Desert], GetSourceRectangle(4)),
                    new ClubSellerMenu(GetBoundsRectangle(5), textures[MenuTabID.Desert], GetSourceRectangle(5))
                });
                break;
            case MenuTabID.GingerIsland:
                options.AddRange(new BaseActiveMenu[]
                {
                    new QiSpecialOrderMenu(GetBoundsRectangle(0), textures[MenuTabID.GingerIsland], GetSourceRectangle(0)),
                    new QiGemShopMenu(GetBoundsRectangle(1), textures[MenuTabID.GingerIsland], GetSourceRectangle(1)),
                    new QiCatMenu(GetBoundsRectangle(2), textures[MenuTabID.GingerIsland], GetSourceRectangle(2), helper),
                    new IslandTradeMenu(GetBoundsRectangle(3), textures[MenuTabID.GingerIsland], GetSourceRectangle(3)),
                    new IslandResortMenu(GetBoundsRectangle(4), textures[MenuTabID.GingerIsland], GetSourceRectangle(4)),
                    new VolcanoShopMenu(GetBoundsRectangle(5), textures[MenuTabID.GingerIsland], GetSourceRectangle(5)),
                    new ForgeMenu(GetBoundsRectangle(6), textures[MenuTabID.GingerIsland], GetSourceRectangle(6))
                });
                break;
            case MenuTabID.SVE:
                break;
            case MenuTabID.RSV:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}