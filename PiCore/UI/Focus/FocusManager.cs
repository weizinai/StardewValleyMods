using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Focus;

/// <summary>
/// 自建焦点图：从 retained 视图树收集可获焦元素，按验证过的几何规则在元素间移动焦点，
/// 取代 vanilla 邻居 ID snap（邻居 ID 不可变、元素移动须手工同步，与组合式视图树不兼容）。
/// 导航规则（HITL 验证，见设计 spec「Focus = self-built focus graph」）：
/// 按请求轴向的半平面内选目标，距离一律用**中心点**（非左上角）度量；左右走同排（Y 区间重叠）优先，
/// 同排无候选再回退全图最近者；等距时交叉轴偏移最小（向下走同列、向上回同列）。上/下在滚动列表容器
/// （<see cref="Scrollable" />）内先走本列表自己的项（自动滚入视口），仅当位于首/末项（该方向已无列表内候选）
/// 才离开列表回退全图。
/// 结构变化（增/删可获焦元素）由宿主在本树结构版本（<see cref="Element.TreeStructureVersion" />）变化时
/// 调用 <see cref="RequestRebuild" /> 触发重建，保留当前焦点（仍存在则保留，已移除则按上次位置就近回退，
/// 尚无焦点则取首个）；重建落点与每次移动后的焦点都会滚入所在滚动容器的视口内。
/// </summary>
public class FocusManager
{
    private readonly Element root;
    private readonly List<Element> items = new();
    private Element? current;

    /// <summary>当前获焦元素（树上无任何可获焦元素时为 null）。</summary>
    public Element? Current => this.current;

    /// <summary>构造焦点图：以 <paramref name="root" /> 为根收集可获焦元素并置初始焦点（首个可获焦元素）。</summary>
    /// <param name="root">布局根节点。</param>
    public FocusManager(Element root)
    {
        this.root = root;
        this.RequestRebuild();
    }

    /// <summary>
    /// 重建焦点图：重新收集树上全部可获焦元素。当前焦点仍存在则保留；已被移除则按上次位置就近回退；
    /// 尚无焦点（首次）则取首个可获焦元素。
    /// </summary>
    public void RequestRebuild()
    {
        var previousCenter = this.current?.Bounds.Center;
        this.items.Clear();
        this.CollectFocusable(this.root, this.items);

        if (this.items.Count == 0)
        {
            this.SetCurrent(null);

            return;
        }

        if (this.current is not null && this.items.Contains(this.current))
        {
            return; // 焦点仍在树上，无需改写
        }

        var fallback = previousCenter is { } center ? this.FindNearest(center) : null;
        var target = fallback ?? this.items[0];
        this.SetCurrent(target);
        this.EnsureVisibleInScrollables(target);
    }

    /// <summary>按几何规则把焦点向 <paramref name="direction" /> 移动一格；已在边界/该方向无目标时不移动。</summary>
    /// <param name="direction">移动方向。</param>
    /// <returns>焦点发生了移动时为 true（宿主据此播导航音并让光标落到新获焦项）。</returns>
    public bool Move(FocusDirection direction)
    {
        if (this.items.Count == 0 || this.current is null)
        {
            return false;
        }

        var next = this.FindNext(this.current, direction);

        if (next is null || ReferenceEquals(next, this.current))
        {
            return false;
        }

        this.SetCurrent(next);
        this.EnsureVisibleInScrollables(next);

        return true;
    }

    /// <summary>把焦点显式设到 <paramref name="item" />（须仍在焦点图中；鼠标点击后让手柄续接从点击处开始）。</summary>
    /// <param name="item">目标元素。</param>
    public void Focus(Element item)
    {
        if (this.items.Contains(item))
        {
            this.SetCurrent(item);
        }
    }

    /// <summary>执行当前获焦元素的激活动作（手柄 A 按下时调用；当前项不可激活则无操作）。</summary>
    public void Activate()
    {
        if (this.current?.ActivateAction is { } action)
        {
            action();
        }
    }

    /// <summary>收集整棵子树中可见且可获焦的元素（先序，顺序即绘制/导航顺序）。</summary>
    /// <param name="node">当前节点。</param>
    /// <param name="result">收集结果。</param>
    private void CollectFocusable(Element node, List<Element> result)
    {
        foreach (var child in node.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            if (child.Focusable)
            {
                result.Add(child);
            }

            this.CollectFocusable(child, result);
        }
    }

    /// <summary>
    /// 按方向选出下一焦点：左右先在同排候选中选（半平面 + 主轴最近 + 等距交叉轴最小），同排无候选再回退全图。
    /// 上下在滚动列表容器内先走本列表自己的项（自动滚入视口），仅当本列表在该方向已无候选（首/末项）才离开列表回退全图。
    /// </summary>
    /// <param name="current">当前获焦元素。</param>
    /// <param name="direction">移动方向。</param>
    /// <returns>下一焦点；该方向无任何候选时为 null。</returns>
    private Element? FindNext(Element current, FocusDirection direction)
    {
        if (direction is FocusDirection.Left or FocusDirection.Right)
        {
            return this.FindBest(current, direction, candidate => SameRow(current, candidate))
                   ?? this.FindBest(current, direction, null);
        }

        // 上/下：当前项在滚动列表容器内时，先只在同列表内选（滚动到列表末项才离开列表）
        var list = Scrollable.FindAncestor(current);

        if (list is not null)
        {
            var inList = this.FindBest(current, direction, list.IsAncestorOf);

            if (inList is not null)
            {
                return inList;
            }
        }

        return this.FindBest(current, direction, null);
    }

    /// <summary>在（可选过滤的）候选中按半平面 + 主轴距离最近 + 等距时交叉轴偏移最小选出最佳目标。</summary>
    /// <param name="current">当前获焦元素。</param>
    /// <param name="direction">移动方向。</param>
    /// <param name="filter">额外候选过滤（null = 不过滤）。</param>
    /// <returns>最佳目标；无候选时为 null。</returns>
    private Element? FindBest(Element current, FocusDirection direction, Func<Element, bool>? filter)
    {
        var center = current.Bounds.Center;
        Element? best = null;
        var bestMain = int.MaxValue;
        var bestCross = int.MaxValue;

        foreach (var candidate in this.items)
        {
            if (ReferenceEquals(candidate, current) || !candidate.Visible || filter is not null && !filter(candidate))
            {
                continue;
            }

            var candidateCenter = candidate.Bounds.Center;

            if (!IsInHalfPlane(center, candidateCenter, direction))
            {
                continue;
            }

            var main = MainAxisDistance(center, candidateCenter, direction);
            var cross = CrossAxisDistance(center, candidateCenter, direction);

            // 布局坐标为整像素，主轴/交叉轴距离恒为整数：整数判等精确，避免浮点相等比较的精度歧义
            if (main < bestMain || main == bestMain && cross < bestCross)
            {
                best = candidate;
                bestMain = main;
                bestCross = cross;
            }
        }

        return best;
    }

    /// <summary>在全部候选中按欧氏距离（平方）选离 <paramref name="center" /> 最近者（焦点被移除时的就近回退）。</summary>
    /// <param name="center">参照中心。</param>
    /// <returns>最近的可获焦元素；无候选时为 null。</returns>
    private Element? FindNearest(Point center)
    {
        Element? best = null;
        var bestDistance = float.MaxValue;

        foreach (var candidate in this.items)
        {
            var candidateCenter = candidate.Bounds.Center;
            float dx = candidateCenter.X - center.X;
            float dy = candidateCenter.Y - center.Y;
            var distance = dx * dx + dy * dy;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }

        return best;
    }

    /// <summary>切换当前获焦元素并同步各元素 <see cref="Element.IsFocused" /> 视觉状态。</summary>
    /// <param name="item">新的获焦元素（null = 清空焦点）。</param>
    private void SetCurrent(Element? item)
    {
        if (ReferenceEquals(item, this.current))
        {
            return;
        }

        if (this.current is not null)
        {
            this.current.IsFocused = false;
        }

        this.current = item;

        if (item is not null)
        {
            item.IsFocused = true;
        }
    }

    /// <summary>
    /// 把获焦项滚入其所在**最近**滚动容器的视口内（上/下两个方向都处理；无滚动祖先时无操作）。
    /// 嵌套滚动容器的多层自动滚入不在票 04 范围（单列表容器已验证），只处理焦点真正所在的列表。
    /// </summary>
    /// <param name="item">获焦元素。</param>
    private void EnsureVisibleInScrollables(Element item)
    {
        Scrollable.FindAncestor(item)?.EnsureVisible(item);
    }

    /// <summary>目标是否落在以当前点为准、沿 <paramref name="direction" /> 的开放半平面内（严格大于/小于，中心同线不算）。</summary>
    private static bool IsInHalfPlane(Point current, Point candidate, FocusDirection direction)
    {
        return direction switch
        {
            FocusDirection.Up => candidate.Y < current.Y,
            FocusDirection.Down => candidate.Y > current.Y,
            FocusDirection.Left => candidate.X < current.X,
            FocusDirection.Right => candidate.X > current.X,
            _ => false
        };
    }

    /// <summary>主轴（移动方向轴）上的距离：左右取 |ΔX|，上下取 |ΔY|（坐标为整像素，距离为整数）。</summary>
    private static int MainAxisDistance(Point current, Point candidate, FocusDirection direction)
    {
        return direction is FocusDirection.Left or FocusDirection.Right
            ? Math.Abs(candidate.X - current.X)
            : Math.Abs(candidate.Y - current.Y);
    }

    /// <summary>交叉轴偏移：左右取 |ΔY|，上下取 |ΔX|（等距时优先最小者，保证向下沿同列/向右沿同行；坐标为整像素，偏移为整数）。</summary>
    private static int CrossAxisDistance(Point current, Point candidate, FocusDirection direction)
    {
        return direction is FocusDirection.Left or FocusDirection.Right
            ? Math.Abs(candidate.Y - current.Y)
            : Math.Abs(candidate.X - current.X);
    }

    /// <summary>两元素是否同排：Y 区间相互重叠（左右同排优先的判定，页签/横排按钮 A→B→C 一格格走）。</summary>
    private static bool SameRow(Element a, Element b)
    {
        return a.Bounds.Y < b.Bounds.Bottom && b.Bounds.Y < a.Bounds.Bottom;
    }
}
