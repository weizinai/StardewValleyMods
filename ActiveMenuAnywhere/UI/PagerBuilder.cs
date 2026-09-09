using System;
using System.Collections.Generic;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.UI;

/// <summary>
/// 页签内容构造器：把一组选项按「每页 9 格、3 列网格」拆成多页 <see cref="Grid" />，
/// 装入 <see cref="Pager" />（内容在上、底部翻页条），并设置翻页按钮的本地化文字。
/// 与旧版自绘菜单每个页签的内容形态（3×3 九宫格、每页 9 个）保持一致。
/// </summary>
internal static class PagerBuilder
{
    /// <summary>每页选项数。</summary>
    private const int OptionsPerPage = 9;

    /// <summary>网格列数。</summary>
    private const int GridColumns = 3;

    /// <summary>网格行数。</summary>
    private const int GridRows = 3;

    /// <summary>内容区与翻页条之间的间距。</summary>
    private const float PagerSpacing = 8f;

    /// <summary>构造一个分页内容：选项条目按固定顺序装入多页 3 列网格。</summary>
    /// <param name="options">该页签的选项条目（目录物化结果）。</param>
    /// <param name="isFavoriteTab">是否收藏页签（决定格子点击的收藏分支）。</param>
    /// <returns>分页内容。</returns>
    public static Pager Build(IReadOnlyList<OptionEntry> options, bool isFavoriteTab)
    {
        var pager = new Pager(spacing: PagerSpacing)
        {
            PreviousButton =
            {
                Text = I18n.UI_Page_Previous()
            },
            NextButton =
            {
                Text = I18n.UI_Page_Next()
            }
        };

        for (var start = 0; start < options.Count; start += OptionsPerPage)
        {
            var page = new Grid(GridColumns, GridRows, OptionTile.CellSize, OptionTile.CellSize);

            for (var i = start; i < Math.Min(start + OptionsPerPage, options.Count); i++)
            {
                page.Add(new OptionTile(options[i], isFavoriteTab));
            }

            pager.AddPage(page);
        }

        return pager;
    }
}