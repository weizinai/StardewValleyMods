using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.PiCore.UI.Host;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;

/// <summary>
/// 主菜单的静态入口：把菜单根视图交给 PiCore UI 框架的 <see cref="MenuHost" /> 打开，
/// 替代旧版自绘的 <see cref="StardewValley.Menus.IClickableMenu" /> 壳。菜单本身（页签/格子/翻页/收藏逻辑）
/// 由 <see cref="MainMenuView" /> 与 <see cref="OptionTile" /> 等框架元素承担；
/// 页签结构由 <see cref="OptionCatalog" /> 目录物化。
/// </summary>
internal static class MenuLauncher
{
    /// <summary>菜单盒宽度（容纳最宽页签栏与 504 宽网格，两侧留白均衡）。</summary>
    private const int MenuWidth = 820;

    /// <summary>菜单盒高度（720p 视口下标题与翻页条完整可见，上下留边距）。</summary>
    private const int MenuHeight = 716;

    /// <summary>打开菜单：构造根视图并交给框架宿主打开。</summary>
    /// <param name="tabId">默认选中的页签 id。</param>
    public static void Open(string tabId)
    {
        var view = new MainMenuView(tabId);
        MenuHost.OpenMenu(view, MenuWidth, MenuHeight);
    }

    /// <summary>菜单当前是否已打开（活动菜单是承载本根视图的框架宿主）。</summary>
    /// <returns>已打开时为 true。</returns>
    public static bool IsOpen()
    {
        return Game1.activeClickableMenu is MenuHost { Root: MainMenuView };
    }
}
