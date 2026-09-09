using System.Collections.Generic;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Config;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;

/// <summary>
/// 菜单根视图：标题 + <see cref="TabControl" />。页签顺序、页签标题与每页签选项列表
/// 全部由 <see cref="OptionCatalog" /> 目录物化（含 SVE/RSV 按模组安装情况的页签行可见性谓词）。
/// 收藏页签（<see cref="OptionCatalog.FavoriteTabId" />）是目录注入的固定第一位特殊页签，
/// 内容继续用每次重建的 <see cref="FavoriteTabContent" />（旧路径）。默认选中
/// <see cref="ModConfig.DefaultMenuTabId" />。
/// </summary>
internal class MainMenuView : Stack
{
    /// <summary>构造菜单根视图。</summary>
    /// <param name="defaultTabId">默认选中的页签 id。</param>
    public MainMenuView(string defaultTabId)
        : base(Direction.Vertical)
    {
        this.Add(new Label("Active Menu Anywhere", center: true));

        var tabs = new TabControl();
        this.Add(tabs);

        var order = OptionCatalog.GetTabOrder();
        foreach (var tabId in order)
        {
            var content = tabId == OptionCatalog.FavoriteTabId
                ? (Element)new FavoriteTabContent()
                : PagerBuilder.Build(OptionCatalog.CreateTabOptions(tabId), isFavoriteTab: false);
            tabs.AddTab(OptionCatalog.GetTabTitle(tabId), content);
        }

        var defaultIndex = IndexOf(order, defaultTabId);
        if (defaultIndex >= 0)
        {
            tabs.Select(defaultIndex);
        }
    }

    /// <summary>在页签顺序里查找目标页签的下标（未列出时返回 -1）。</summary>
    /// <param name="order">页签顺序。</param>
    /// <param name="tabId">目标页签 id。</param>
    /// <returns>目标页签在顺序中的下标。</returns>
    private static int IndexOf(IReadOnlyList<string> order, string tabId)
    {
        for (var i = 0; i < order.Count; i++)
        {
            if (order[i] == tabId)
            {
                return i;
            }
        }

        return -1;
    }
}
