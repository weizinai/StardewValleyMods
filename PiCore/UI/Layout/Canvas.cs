using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 绝对定位容器：子级自带位置偏移，按“子级左上角 + 容器左上角”放置。逃逸自动布局的出口。
/// 本文件为已验证原型（UiFrameworkProto）的移植；相对原型额外在 <see cref="Remove" /> / <see cref="Clear" />
/// 时同步清理偏移表（基类移除子级时不会通知容器）。
/// </summary>
public class Canvas : Element
{
    private readonly Dictionary<Element, Vector2> offsets = new();

    /// <summary>设置子级绝对位置（相对本容器左上角）。</summary>
    /// <param name="child">子元素。</param>
    /// <param name="localPosition">相对本容器左上角的偏移。</param>
    public void SetChildPosition(Element child, Vector2 localPosition)
    {
        this.offsets[child] = localPosition;
        this.MarkDirty();
    }

    /// <summary>取子级位置偏移，未设置过时返回零。</summary>
    /// <param name="child">子元素。</param>
    /// <returns>相对本容器左上角的偏移。</returns>
    public Vector2 GetChildPosition(Element child)
    {
        return this.offsets.TryGetValue(child, out var value) ? value : Vector2.Zero;
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        var max = Vector2.Zero;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            child.Measure(available);
            var pos = this.GetChildPosition(child);
            max.X = Math.Max(max.X, pos.X + child.DesiredSize.X);
            max.Y = Math.Max(max.Y, pos.Y + child.DesiredSize.Y);
        }

        return max;
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            var pos = this.GetChildPosition(child);
            child.Arrange(new Rectangle(
                final.X + (int)pos.X,
                final.Y + (int)pos.Y,
                (int)Math.Min(child.DesiredSize.X, final.Width),
                (int)Math.Min(child.DesiredSize.Y, final.Height)));
        }
    }

    /// <inheritdoc />
    public override void Remove(Element child)
    {
        base.Remove(child);
        this.offsets.Remove(child);
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        this.offsets.Clear();
    }
}
