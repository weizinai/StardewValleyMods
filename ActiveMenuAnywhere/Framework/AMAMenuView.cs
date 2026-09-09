using System;
using System.Collections.Generic;
using StardewModdingAPI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

/// <summary>
/// 菜单根视图：标题 + <see cref="TabControl" />。页签顺序与原版一致
/// （收藏/农场/小镇/深山/森林/沙滩/沙漠/姜岛），SVE 与 RSV 按是否安装对应模组条件挂载。
/// 每个页签内容为 <see cref="Pager" />（收藏页签用每次重建的 <see cref="FavoriteTabContent" />），
/// 切换到页签时自动回到第一页。默认选中 <see cref="ModConfig.DefaultMenuTabId" />。
/// </summary>
internal class AMAMenuView : Stack
{
    /// <summary>标题与页签栏之间的间距。</summary>
    private const float Spacing = 8f;

    /// <summary>SVE 模组唯一标识。</summary>
    private const string SveModId = "FlashShifter.SVECode";

    /// <summary>RSV 模组唯一标识。</summary>
    private const string RsvModId = "Rafseazz.RidgesideVillage";

    /// <summary>构造菜单根视图。</summary>
    /// <param name="defaultTabId">默认选中的页签。</param>
    /// <param name="helper">SMAPI 帮助器（用于检测 SVE/RSV 模组）。</param>
    public AMAMenuView(MenuTabId defaultTabId, IModHelper helper)
        : base(Direction.Vertical, Spacing)
    {
        this.Add(new Label("Active Menu Anywhere", center: true));

        var tabs = new TabControl();
        this.Add(tabs);

        var order = this.CreateTabOrder(helper);

        foreach (var tabId in order)
        {
            var title = this.GetTabTitle(tabId);
            var content = tabId == MenuTabId.Favorite
                ? (Element)new FavoriteTabContent()
                : PagerBuilder.Build(AMAMenuView.CreateOptions(tabId), isFavoriteTab: false);
            tabs.AddTab(title, content);
        }

        var defaultIndex = order.IndexOf(defaultTabId);
        if (defaultIndex >= 0)
        {
            tabs.Select(defaultIndex);
        }
    }

    /// <summary>构造页签顺序：固定页签按原版顺序，SVE/RSV 依安装状态追加。</summary>
    /// <param name="helper">SMAPI 帮助器。</param>
    /// <returns>页签顺序列表。</returns>
    private List<MenuTabId> CreateTabOrder(IModHelper helper)
    {
        var order = new List<MenuTabId>
        {
            MenuTabId.Favorite,
            MenuTabId.Farm,
            MenuTabId.Town,
            MenuTabId.Mountain,
            MenuTabId.Forest,
            MenuTabId.Beach,
            MenuTabId.Desert,
            MenuTabId.GingerIsland
        };

        if (helper.ModRegistry.Get(SveModId) is not null)
        {
            order.Add(MenuTabId.SVE);
        }

        if (helper.ModRegistry.Get(RsvModId) is not null)
        {
            order.Add(MenuTabId.RSV);
        }

        return order;
    }

    /// <summary>取页签标题文案。</summary>
    /// <param name="tabId">页签标识。</param>
    /// <returns>页签标题。</returns>
    private string GetTabTitle(MenuTabId tabId)
    {
        return tabId switch
        {
            MenuTabId.Favorite => I18n.UI_Tab_Favorites(),
            MenuTabId.Farm => I18n.UI_Tab_Farm(),
            MenuTabId.Town => I18n.UI_Tab_Town(),
            MenuTabId.Mountain => I18n.UI_Tab_Mountain(),
            MenuTabId.Forest => I18n.UI_Tab_Forest(),
            MenuTabId.Beach => I18n.UI_Tab_Beach(),
            MenuTabId.Desert => I18n.UI_Tab_Desert(),
            MenuTabId.GingerIsland => I18n.UI_Tab_GingerIsland(),
            MenuTabId.SVE => I18n.UI_Tab_SVE(),
            MenuTabId.RSV => I18n.UI_Tab_RSV(),
            _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
        };
    }

    /// <summary>按页签标识创建对应选项集合。</summary>
    /// <param name="tabId">页签标识。</param>
    /// <returns>该页签的选项。</returns>
    private static BaseOption[] CreateOptions(MenuTabId tabId)
    {
        return tabId switch
        {
            MenuTabId.Farm => OptionFactory.CreateFarmOptions(),
            MenuTabId.Town => OptionFactory.CreateTownOptions(),
            MenuTabId.Mountain => OptionFactory.CreateMountainOptions(),
            MenuTabId.Forest => OptionFactory.CreateForestOptions(),
            MenuTabId.Beach => OptionFactory.CreateBeachOptions(),
            MenuTabId.Desert => OptionFactory.CreateDesertOptions(),
            MenuTabId.GingerIsland => OptionFactory.CreateGingerIslandOptions(),
            MenuTabId.SVE => OptionFactory.CreateSVEOptions(),
            MenuTabId.RSV => OptionFactory.CreateRSVOptions(),
            _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
        };
    }
}
