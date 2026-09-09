using StardewModdingAPI;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI.Host;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

/// <summary>
/// AMAMenu 的静态入口：把菜单根视图交给 PiCore UI 框架的 <see cref="MenuHost" /> 打开，
/// 替代旧版自绘的 <see cref="IClickableMenu" /> 壳。菜单本身（页签/格子/翻页/收藏逻辑）
/// 由 <see cref="AMAMenuView" /> 与 <see cref="OptionTile" /> 等框架元素承担。
/// </summary>
internal static class AMAMenu
{
    /// <summary>菜单盒宽度（容纳最宽页签栏与 504 宽网格，两侧留白均衡）。</summary>
    private const int MenuWidth = 820;

    /// <summary>菜单盒高度（720p 视口下标题与翻页条完整可见，上下留边距）。</summary>
    private const int MenuHeight = 716;

    /// <summary>打开菜单：构造根视图并交给框架宿主打开。</summary>
    /// <param name="menuTabId">默认选中的页签。</param>
    /// <param name="helper">SMAPI 帮助器。</param>
    public static void Open(MenuTabId menuTabId, IModHelper helper)
    {
        var view = new AMAMenuView(menuTabId, helper);
        MenuHost.OpenMenu(view, MenuWidth, MenuHeight);
    }

    /// <summary>菜单当前是否已打开（活动菜单是承载本根视图的框架宿主）。</summary>
    /// <returns>已打开时为 true。</returns>
    public static bool IsOpen()
    {
        return Game1.activeClickableMenu is MenuHost menu && menu.Root is AMAMenuView;
    }
}
