using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

public class AMAMenu : IClickableMenu
{
    private const int InnerWidth = 600;
    private const int InnerHeight = 600;

    private const int OptionsPerPage = 9;
    private readonly ModConfig config = ModConfig.Instance;
    private readonly IModHelper helper;

    private readonly (int x, int y) innerDrawPosition =
        (x: Game1.uiViewport.Width / 2 - InnerWidth / 2, y: Game1.uiViewport.Height / 2 - InnerHeight / 2);

    private readonly List<BaseOption> options = new();
    private readonly List<ClickableComponent> optionSlots = new();
    private readonly List<ClickableComponent> tabs = new();

    private MenuTabId currentMenuTabId;

    private int currentPage;
    private ClickableTextureComponent downArrow = null!;
    private ClickableTextureComponent upArrow = null!;

    public AMAMenu(MenuTabId menuTabId, IModHelper helper)
    {
        this.helper = helper;
        this.Init(menuTabId);
        this.ResetComponents();
        this.SetOptions();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // arrow
        if (this.upArrow.containsPoint(x, y) && this.currentPage > 0) this.currentPage--;
        if (this.downArrow.containsPoint(x, y) && this.options.Count - (this.currentPage + 1) * OptionsPerPage > 0) this.currentPage++;

        // tab
        var tab = this.tabs.FirstOrDefault(tab => tab.containsPoint(x, y));
        if (tab != null) Game1.activeClickableMenu = new AMAMenu(this.GetTabId(tab), this.helper);

        // option
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + this.currentPage * OptionsPerPage;
            if (this.optionSlots[i].containsPoint(x, y) && optionsIndex < this.options.Count)
            {
                var option = this.options[optionsIndex];
                if (this.config.FavoriteKey.IsDown())
                {
                    if (this.currentMenuTabId == MenuTabId.Favorite)
                    {
                        this.config.FavoriteMenus.Remove(option.Id);
                        Logger.NoIconHUDMessage(I18n.UI_Favorite_Remove(), 1000);
                    }
                    else
                    {
                        if (!this.config.FavoriteMenus.Contains(option.Id))
                        {
                            this.config.FavoriteMenus.Add(option.Id);
                            Logger.NoIconHUDMessage(I18n.UI_Favorite_Add(), 1000);
                        }
                        else
                        {
                            Logger.NoIconHUDMessage(I18n.UI_Favorite_Exist(), 1000);
                        }
                    }
                }
                else
                {
                    if (option.IsEnable() || !this.config.ProgressMode)
                        option.Apply();
                    else
                        Game1.drawObjectDialogue(I18n.UI_Tip_Unavailable());
                }
                break;
            }
        }
    }

    public override void performHoverAction(int x, int y)
    {
        this.upArrow.tryHover(x, y);
        this.downArrow.tryHover(x, y);

        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionIndex = i + this.currentPage * OptionsPerPage;
            if (optionIndex >= this.options.Count) return;
            this.options[optionIndex].Scale = this.optionSlots[i].containsPoint(x, y) ? 0.9f : 1f;
        }
    }

    public override void draw(SpriteBatch b)
    {
        // Draw background
        drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

        // Draw title
        SpriteText.drawStringWithScrollCenteredAt(b, "Active Menu Anywhere", this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen - 64);

        // Draw arrows
        this.upArrow.draw(b);
        this.downArrow.draw(b);

        // Draw tabs
        this.tabs.ForEach(tab => DrawHelper.DrawTab(
            tab.bounds.X + tab.bounds.Width,
            tab.bounds.Y,
            Game1.smallFont,
            tab.name,
            Align.Right,
            this.GetTabId(tab) == this.currentMenuTabId ? 0.7f : 1f
        ));

        // Draw options
        this.DrawOption(b);

        // Draw Mouse
        this.drawMouse(b);
    }

    private void Init(MenuTabId menuTabId)
    {
        this.width = InnerWidth + borderWidth * 2;
        this.height = InnerHeight + borderWidth * 2;
        this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
        this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2;

        this.currentMenuTabId = menuTabId;
    }

    private void ResetComponents()
    {
        // Add arrows
        this.AddArrows();

        // Add tabs
        this.AddTabs();

        // Add optionSlots
        this.AddOptionSlots();
    }

    private MenuTabId GetTabId(ClickableComponent tab)
    {
        return Enum.Parse<MenuTabId>(tab.label);
    }

    private Rectangle GetBoundsRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;
        return new Rectangle(this.innerDrawPosition.x + i * 200, this.innerDrawPosition.y + j * 200, 200, 200);
    }

    private Rectangle GetTabRectangle(int index)
    {
        var tabOffset = (x: 4, y: 16);
        var tabSize = (width: 100, height: 48);
        var tabPosition = (x: this.xPositionOnScreen - tabSize.width, y: this.yPositionOnScreen + tabOffset.y);

        return new Rectangle(tabPosition.x, tabPosition.y + tabSize.height * index, tabSize.width - tabOffset.x, tabSize.height);
    }

    private void AddArrows()
    {
        const float scale = 4f;
        var offset = (x: 8, y: (int)(12 * scale / 2));
        this.upArrow = new ClickableTextureComponent(
            new Rectangle(
                this.xPositionOnScreen + this.width + offset.x,
                this.yPositionOnScreen - offset.y,
                (int)(11 * scale),
                (int)(12 * scale)
            ),
            Game1.mouseCursors,
            new Rectangle(421, 459, 11, 12),
            scale
        );
        this.downArrow = new ClickableTextureComponent(
            new Rectangle(
                this.xPositionOnScreen + this.width + offset.x,
                this.yPositionOnScreen + this.height - offset.y,
                (int)(11 * scale),
                (int)(12 * scale)
            ),
            Game1.mouseCursors,
            new Rectangle(421, 472, 11, 12),
            scale
        );
    }

    private void AddTabs()
    {
        var i = 0;
        this.tabs.Clear();
        this.tabs.AddRange(new[]
        {
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Favorites(), nameof(MenuTabId.Favorite)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Farm(), nameof(MenuTabId.Farm)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Town(), nameof(MenuTabId.Town)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Mountain(), nameof(MenuTabId.Mountain)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Forest(), nameof(MenuTabId.Forest)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Beach(), nameof(MenuTabId.Beach)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_Desert(), nameof(MenuTabId.Desert)),
            new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_GingerIsland(), nameof(MenuTabId.GingerIsland))
        });

        if (this.helper.ModRegistry.Get("FlashShifter.SVECode") != null)
        {
            this.tabs.Add(new ClickableComponent(this.GetTabRectangle(i++), I18n.UI_Tab_SVE(), nameof(MenuTabId.SVE)));
        }

        if (this.helper.ModRegistry.Get("Rafseazz.RidgesideVillage") != null)
        {
            this.tabs.Add(new ClickableComponent(this.GetTabRectangle(i), I18n.UI_Tab_RSV(), nameof(MenuTabId.RSV)));
        }
    }

    private void SetOptions()
    {
        this.options.Clear();
        switch (this.currentMenuTabId)
        {
            case MenuTabId.Favorite:
                this.options.AddRange(OptionFactory.CreateFavoriteOptions());
                break;
            case MenuTabId.Farm:
                this.options.AddRange(OptionFactory.CreateFarmOptions());
                break;
            case MenuTabId.Town:
                this.options.AddRange(OptionFactory.CreateTownOptions());
                break;
            case MenuTabId.Mountain:
                this.options.AddRange(OptionFactory.CreateMountainOptions());
                break;
            case MenuTabId.Forest:
                this.options.AddRange(OptionFactory.CreateForestOptions());
                break;
            case MenuTabId.Beach:
                this.options.AddRange(OptionFactory.CreateBeachOptions());
                break;
            case MenuTabId.Desert:
                this.options.AddRange(OptionFactory.CreateDesertOptions());
                break;
            case MenuTabId.GingerIsland:
                this.options.AddRange(OptionFactory.CreateGingerIslandOptions());
                break;
            case MenuTabId.SVE:
                this.options.AddRange(OptionFactory.CreateSVEOptions());
                break;
            case MenuTabId.RSV:
                this.options.AddRange(OptionFactory.CreateRSVOptions());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddOptionSlots()
    {
        for (var i = 0; i < OptionsPerPage; i++)
        {
            this.optionSlots.Add(new ClickableComponent(this.GetBoundsRectangle(i), ""));
        }
    }

    private void DrawOption(SpriteBatch b)
    {
        for (var i = 0; i < OptionsPerPage; i++)
        {
            var optionsIndex = i + this.currentPage * OptionsPerPage;
            var bounds = this.optionSlots[i].bounds;
            if (optionsIndex >= this.options.Count) break;
            this.options[optionsIndex].Draw(b, bounds.X, bounds.Y);
        }
    }
}