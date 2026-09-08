using System;
using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 沿主轴顺序排布子级的容器（垂直或水平，内容自适应）。交叉轴默认撑满，主轴取子级期望尺寸之和。
/// 本文件为已验证原型（UiFrameworkProto）的移植，未改动其布局语义。
/// </summary>
public class Stack : Element
{
    /// <summary>排布方向。</summary>
    public enum Direction
    {
        /// <summary>垂直（自上而下）。</summary>
        Vertical,

        /// <summary>水平（自左而右）。</summary>
        Horizontal
    }

    private readonly Direction direction;
    private readonly float spacing;

    /// <summary>构造堆叠容器。</summary>
    /// <param name="direction">排布方向。</param>
    /// <param name="spacing">相邻子级间隔。</param>
    public Stack(Direction direction, float spacing = 8f)
    {
        this.direction = direction;
        this.spacing = spacing;
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        var main = 0f;
        var cross = 0f;
        var visible = 0;

        // 主轴按“完整可用”给每个子级测量（不递减预算）：Stack 子级取自然期望尺寸，
        // 容器最终期望尺寸再按主轴可用空间封顶。超出的内容交由上层 Scrollable/Canvas 裁剪或溢出。
        var availableMain = this.direction == Direction.Vertical ? available.Y : available.X;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            var slot = this.direction == Direction.Vertical
                ? new Vector2(available.X, availableMain)
                : new Vector2(availableMain, available.Y);
            var desired = child.Measure(slot);
            main += this.direction == Direction.Vertical ? desired.Y : desired.X;
            cross = Math.Max(cross, this.direction == Direction.Vertical ? desired.X : desired.Y);
            visible++;
        }

        main += Math.Max(0, visible - 1) * this.spacing;
        var totalMain = Math.Min(main, availableMain);

        return this.direction == Direction.Vertical
            ? new Vector2(cross, totalMain)
            : new Vector2(totalMain, cross);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        float cursor = this.direction == Direction.Vertical ? final.Y : final.X;
        float availableMain = this.direction == Direction.Vertical ? final.Height : final.Width;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            var main = this.direction == Direction.Vertical ? child.DesiredSize.Y : child.DesiredSize.X;

            // 每个槽位主轴以自然期望为主，但受“从光标到容器底”的剩余空间约束（防越界）
            var remaining = availableMain - (cursor - (this.direction == Direction.Vertical ? final.Y : final.X));
            var slotMain = Math.Max(0f, Math.Min(main, remaining));
            var slot = this.direction == Direction.Vertical
                ? new Rectangle(final.X, (int)cursor, final.Width, (int)slotMain)
                : new Rectangle((int)cursor, final.Y, (int)slotMain, final.Height);
            child.Arrange(slot);
            cursor += this.direction == Direction.Vertical ? slot.Height + this.spacing : slot.Width + this.spacing;
        }
    }
}
