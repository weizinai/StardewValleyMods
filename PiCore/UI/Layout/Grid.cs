using System;
using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 把子级排成固定行/列、格子等大的容器（如 AMAMenu 的 3×3 九宫格）。
/// 整块（cols×cellW × rows×cellH）在分配区内居中，避免“左右不居中”。
/// </summary>
public class Grid : Element
{
    private readonly int columns;
    private readonly int rows;
    private readonly float cellWidth;
    private readonly float cellHeight;

    /// <summary>构造网格容器。</summary>
    /// <param name="columns">列数。</param>
    /// <param name="rows">行数。</param>
    /// <param name="cellWidth">格子宽。</param>
    /// <param name="cellHeight">格子高。</param>
    public Grid(int columns, int rows, float cellWidth, float cellHeight)
    {
        this.columns = columns;
        this.rows = rows;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        foreach (var child in this.Children)
        {
            child.Measure(new Vector2(this.cellWidth, this.cellHeight));
        }

        return new Vector2(this.columns * this.cellWidth, this.rows * this.cellHeight);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 网格块（整块 cols*cellW x rows*cellH）在分配区内居中，避免“左右不居中”。
        var blockWidth = this.columns * this.cellWidth;
        var blockHeight = this.rows * this.cellHeight;
        var startX = final.X + Math.Max(0f, (final.Width - blockWidth) / 2f);
        var startY = final.Y + Math.Max(0f, (final.Height - blockHeight) / 2f);

        var index = 0;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                index++;

                continue;
            }

            var col = index % this.columns;
            var row = index / this.columns;
            var cell = new Rectangle(
                (int)(startX + col * this.cellWidth),
                (int)(startY + row * this.cellHeight),
                (int)this.cellWidth,
                (int)this.cellHeight);
            child.Arrange(cell);
            index++;
        }
    }
}
