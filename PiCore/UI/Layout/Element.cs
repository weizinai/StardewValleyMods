using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 布局树中的节点（retained 内核的根视图类型）。采用 WPF 风格的两趟布局：
/// <see cref="Measure" /> 把可用约束向下传、期望尺寸向上汇总，<see cref="Arrange" />
/// 为每个子级算出最终屏幕空间矩形。任何内容/结构变化经 <see cref="MarkDirty" />
/// 沿父链标脏到根，由 <see cref="LayoutRunner" /> 在宿主每帧统一冲刷。
/// </summary>
public abstract class Element
{
    private readonly List<Element> children = new();

    private Element? parent;

    /// <summary>
    /// 本树结构版本号：<see cref="Add" /> / <see cref="Remove" /> / <see cref="Clear" /> 每次成功变更时
    /// 把**所属树的根**版本 +1（每棵树独立计数，互不干扰）。宿主据此感知“本菜单树的可获焦元素增删”
    /// 并请求焦点图重建（重建保留当前焦点）。
    /// </summary>
    internal int TreeStructureVersion { get; private set; }

    /// <summary>所属树的根（沿父链上溯到顶；未挂父级时即自身）。</summary>
    private Element TreeRoot
    {
        get
        {
            var node = this;

            while (node.parent is not null)
            {
                node = node.parent;
            }

            return node;
        }
    }

    /// <summary>子元素（只读视图）。</summary>
    public IReadOnlyList<Element> Children => this.children;

    /// <summary>父元素。</summary>
    public Element? Parent => this.parent;

    /// <summary>屏幕空间最终矩形（<see cref="Arrange" /> 后有效，滚动已折算进坐标）。</summary>
    public Rectangle Bounds { get; protected set; }

    /// <summary>上次 <see cref="Measure" /> 得到的期望尺寸。</summary>
    public Vector2 DesiredSize { get; protected set; }

    /// <summary>可见性：为 false 时跳过绘制与命中；是否跳过布局由各容器在 measure/arrange 内自行决定。</summary>
    public bool Visible { get; set; } = true;

    /// <summary>附加数据（网格项 id / 焦点组等，框架不解释其含义）。</summary>
    public object? Tag { get; set; }

    /// <summary>提示框文本：鼠标悬停/手柄获焦时由宿主绘制提示框（null/空 = 无提示框）。不参与布局。</summary>
    public string? TooltipText { get; set; }

    /// <summary>
    /// 是否可获焦：为 true 时进入焦点图的候选（控制器摇杆/方向键可导航到它）。
    /// 默认 false（容器/标签等不可获焦）；<see cref="Widget.Button" /> 等可交互组件覆写为 true。
    /// </summary>
    public virtual bool Focusable => false;

    /// <summary>焦点激活动作（A 按下时执行；null = 只可获焦不可激活）。默认 null；Button 把 <see cref="Widget.Button.OnClick" /> 兼作激活。</summary>
    public virtual Action? ActivateAction => null;

    /// <summary>当前是否为焦点图的获焦项（框架内部维护，组件据此绘制获焦视觉）。</summary>
    internal bool IsFocused { get; set; }

    /// <summary>脏标定：本节点或其后代需要重新布局。</summary>
    internal bool Dirty { get; set; } = true;

    /// <summary>添加子元素并建立父子关系，整条路径标脏。</summary>
    /// <param name="child">要添加的子元素。</param>
    /// <exception cref="InvalidOperationException">把元素自身/已挂到别的父级的元素/本元素祖先加为子级时抛出。</exception>
    public void Add(Element child)
    {
        if (child == this)
        {
            throw new InvalidOperationException("不能把元素自身添加为子元素。");
        }

        if (child.parent is not null)
        {
            throw new InvalidOperationException("子元素已属于其他父元素，需先从原父元素移除。");
        }

        for (var ancestor = this.parent; ancestor is not null; ancestor = ancestor.parent)
        {
            if (ancestor == child)
            {
                throw new InvalidOperationException("不能把本元素的祖先添加为子元素，会形成环。");
            }
        }

        this.children.Add(child);
        child.parent = this;
        this.TreeRoot.TreeStructureVersion++;
        this.MarkDirty();
    }

    /// <summary>移除子元素并断开父子关系，整条路径标脏。派生容器可覆写以同步清理按子级记录的状态（如 <see cref="Canvas" /> 的偏移表）。</summary>
    /// <param name="child">要移除的子元素。</param>
    public virtual void Remove(Element child)
    {
        if (this.children.Remove(child))
        {
            child.parent = null;
            this.TreeRoot.TreeStructureVersion++;
            this.MarkDirty();
        }
    }

    /// <summary>清空全部子元素并断开父子关系。派生容器可覆写以同步清理按子级记录的状态（如 <see cref="Canvas" /> 的偏移表）。</summary>
    public virtual void Clear()
    {
        foreach (var child in this.children)
        {
            child.parent = null;
        }

        this.children.Clear();
        this.TreeRoot.TreeStructureVersion++;
        this.MarkDirty();
    }

    /// <summary>内容/数据变化导致需要重新布局时调用（沿父链传播到根）。</summary>
    public void MarkDirty()
    {
        this.Dirty = true;
        this.parent?.MarkDirty();
    }

    /// <summary>在 <paramref name="available" /> 约束下测量自身期望尺寸，并递归测量子级（父容器在自身 MeasureOverride 内调用）。</summary>
    /// <param name="available">本次布局可用的空间。</param>
    /// <returns>本节点的期望尺寸。</returns>
    public Vector2 Measure(Vector2 available)
    {
        this.DesiredSize = this.MeasureOverride(available);

        return this.DesiredSize;
    }

    /// <summary>在 <paramref name="final" /> 内布置自身并递归布置子级（父容器在自身 ArrangeOverride 内调用）。</summary>
    /// <param name="final">本节点最终占用的屏幕空间矩形。</param>
    public void Arrange(Rectangle final)
    {
        this.Bounds = final;
        this.ArrangeOverride(final);
    }

    /// <summary>子类测量实现。</summary>
    /// <param name="available">本次布局可用的空间。</param>
    /// <returns>本节点的期望尺寸。</returns>
    protected abstract Vector2 MeasureOverride(Vector2 available);

    /// <summary>子类布置实现：为每个子级算出 finalChild 并调用 child.Arrange。</summary>
    /// <param name="final">本节点最终占用的屏幕空间矩形。</param>
    protected abstract void ArrangeOverride(Rectangle final);

    /// <summary>绘制自身内容（不含子级）。</summary>
    /// <param name="batch">精灵批。</param>
    protected virtual void DrawSelf(SpriteBatch batch) { }

    /// <summary>绘制自身及可见子级。子类可覆写以做裁剪/变换后自行调用子级 Draw。</summary>
    /// <param name="batch">精灵批。</param>
    public virtual void Draw(SpriteBatch batch)
    {
        if (!this.Visible)
        {
            return;
        }

        this.DrawSelf(batch);

        foreach (var child in this.children)
        {
            if (child.Visible)
            {
                child.Draw(batch);
            }
        }
    }

    /// <summary>点是否落在本元素最终矩形内，且未因祖先 <see cref="Scrollable" /> 的 scissor 裁剪而不可见。</summary>
    /// <param name="point">要测试的点。</param>
    /// <returns>点在本元素矩形内、本元素可见、且落在所有祖先滚动容器内容区内时为 true。</returns>
    public virtual bool ContainsPoint(Point point)
    {
        return this.Visible && this.Bounds.Contains(point) && this.IsWithinViewports(point);
    }

    /// <summary>
    /// 点是否落在每个祖先 <see cref="Scrollable" /> 的内容区（<see cref="Scrollable.InnerViewport" />）内。滚动容器绘制时用 scissor 把内容裁剪在内容区内，
    /// 因此滚出内容区的子级（Bounds 仍是真实屏幕坐标，可能叠到边框或视口外的元素上）对鼠标/提示框是不可见也不可点的——命中必须同时落在
    /// 全部祖先滚动容器内容区内，否则长列表向下滚后滚出的项会挡住其上方（如页签栏）的点击。
    /// </summary>
    /// <param name="point">要测试的点。</param>
    /// <returns>点落在全部祖先滚动容器内容区内（或无滚动容器祖先）时为 true。</returns>
    private bool IsWithinViewports(Point point)
    {
        for (var ancestor = this.parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if (ancestor is Scrollable scrollable && !scrollable.InnerViewport.Contains(point))
            {
                return false;
            }
        }

        return true;
    }
}
