using System;
using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 两趟布局的宿主入口。任何读取元素 <see cref="Element.Bounds" /> 做屏幕定位的代码，
/// 必须先经 <see cref="UpdateIfDirty(Element, Rectangle)" /> 把挂起的脏布局同步落地，否则读到的是滚动前/重排前的旧坐标。
/// </summary>
public static class LayoutRunner
{
    /// <summary>若根已标脏则做一次完整 measure+arrange 并清脏；未标脏则不做任何事。</summary>
    /// <param name="root">布局根节点。</param>
    /// <param name="final">根节点最终占用的屏幕空间矩形。</param>
    public static void UpdateIfDirty(Element root, Rectangle final)
    {
        if (!root.Dirty)
        {
            return;
        }

        root.Measure(new Vector2(final.Width, final.Height));
        root.Arrange(final);
        ClearDirty(root);
    }

    /// <summary>
    /// 若根已标脏则做一次内容自适应布局：先用 <paramref name="available" /> 测量根取期望尺寸，
    /// 再由 <paramref name="place" /> 把期望尺寸换算成最终屏幕矩形并布置、清脏。供内容自适应、不铺满整个
    /// 可用区的宿主使用（如只读 drawable 宿主：根按自身内容尺寸排布，再按锚点摆到屏幕上）。
    /// </summary>
    /// <param name="root">布局根节点。</param>
    /// <param name="available">本次测量可用的空间（内容超界/折行的上限）。</param>
    /// <param name="place">把期望尺寸换算成最终屏幕矩形的回调。</param>
    public static void UpdateIfDirty(Element root, Vector2 available, Func<Vector2, Rectangle> place)
    {
        if (!root.Dirty)
        {
            return;
        }

        root.Measure(available);
        var final = place(root.DesiredSize);
        root.Arrange(final);
        ClearDirty(root);
    }

    /// <summary>无条件强制一次完整布局（首帧或内容整体重建时用）。</summary>
    /// <param name="root">布局根节点。</param>
    /// <param name="final">根节点最终占用的屏幕空间矩形。</param>
    public static void Force(Element root, Rectangle final)
    {
        root.MarkDirty();
        UpdateIfDirty(root, final);
    }

    /// <summary>无条件强制一次内容自适应布局（首帧或内容整体重建时用；布局语义见内容自适应重载）。</summary>
    /// <param name="root">布局根节点。</param>
    /// <param name="available">本次测量可用的空间（内容超界/折行的上限）。</param>
    /// <param name="place">把期望尺寸换算成最终屏幕矩形的回调。</param>
    public static void Force(Element root, Vector2 available, Func<Vector2, Rectangle> place)
    {
        root.MarkDirty();
        UpdateIfDirty(root, available, place);
    }

    private static void ClearDirty(Element node)
    {
        node.Dirty = false;

        foreach (var child in node.Children)
        {
            ClearDirty(child);
        }
    }
}
