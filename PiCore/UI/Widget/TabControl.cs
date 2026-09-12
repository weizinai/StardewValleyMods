using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 页签容器（TabControl）：
/// 顶部一横排页签芯片（可获焦、鼠标/手柄可激活），下方是**当前选中页签的内容区**。每次选择把
/// 上一个页签的内容从树中移除、把新页签的内容挂入（Add/Remove 触发结构版本变化 → 宿主自动重建
/// 焦点图，保留仍存在的焦点），内容区不留残影/错位；若新内容实现 <see cref="IResettable" />（如
/// <see cref="Pager" />）则在挂入后自动 <see cref="IResettable.Reset" />——即“切到该页签回到第一页”。
/// 布局语义：页签栏在顶部（横向排布、高度取芯片自然高），内容区在页签栏下方占满剩余可用区
/// （内容本身再自行居中/铺排，如 <see cref="Grid" /> 整块居中、<see cref="Scrollable" /> 占满视口）。
/// 页签栏内芯片同一排 → 左右导航按“同排优先”一格格走 A→B→C，不会跳过到下方内容；该同排优先依赖
/// 芯片行与内容行不在同一 Y 带（本控件把内容固定在页签栏下方，消费方不应让内容与芯片行重叠）。
/// </summary>
public class TabControl : Element
{
    /// <summary>页签栏与内容区之间的默认间距。</summary>
    private const float DefaultContentSpacing = 10f;

    /// <summary>页签栏里相邻芯片的默认间距。</summary>
    private const float DefaultRailSpacing = 6f;

    /// <summary>
    /// 页签芯片：按钮外观 + “选中”半透明标示（复用 <see cref="Button" /> 的悬停/获焦视觉；
    /// 选中以降低盒子透明度表达，与其余页签在底色明暗上区分）。
    /// </summary>
    private sealed class TabChip : Button
    {
        /// <summary>选中态盒子透明度（参照 AMAMenu 当前页签的 0.7 半透明惯例：选中更透、其余不透明）。</summary>
        private const float SelectedOpacity = 0.7f;

        /// <summary>文本左右内边距（芯片最小宽度 = 文本宽 + 2×该值）。</summary>
        private const float HorizontalPadding = 12f;

        /// <summary>文本上下内边距（芯片最小高度 = 文本高 + 2×该值）。</summary>
        private const float VerticalPadding = 6f;

        /// <summary>构造页签芯片。</summary>
        /// <param name="text">芯片文字。</param>
        public TabChip(string text) : base(text) { }

        /// <summary>是否处于选中态（决定是否用半透明标示；由 <see cref="TabControl" /> 维护，本类外部只读）。</summary>
        public bool Selected { get; internal set; }

        /// <summary>选中页签盒子降为半透明（未选中保持不透明）。</summary>
        /// <inheritdoc />
        protected override float BoxOpacity => this.Selected ? SelectedOpacity : base.BoxOpacity;

        /// <summary>以紧凑内边距测量芯片（页签栏芯片比普通按钮小一圈）。</summary>
        /// <inheritdoc />
        protected override Vector2 MeasureOverride(Vector2 available)
        {
            var size = Theme.SmallFont.MeasureString(this.Text);
            var width = Math.Min(size.X + HorizontalPadding * 2, available.X);
            var height = Math.Min(size.Y + VerticalPadding * 2, available.Y);

            return new Vector2(width, height);
        }
    }

    private readonly Stack rail;
    private readonly List<Element> contents = new();
    private readonly List<TabChip> chips = new();
    private readonly float contentSpacing;

    /// <summary>当前选中的页签索引（-1 = 尚未选中任何页签）。</summary>
    public int SelectedIndex { get; private set; } = -1;

    /// <summary>页签数量。</summary>
    public int TabCount => this.contents.Count;

    /// <summary>当前选中页签的内容元素（未选中时 null）。</summary>
    public Element? SelectedContent => this.SelectedIndex >= 0 ? this.contents[this.SelectedIndex] : null;

    /// <summary>构造页签容器（页签栏在顶部；尚无任何页签时高度为 0）。</summary>
    /// <param name="railSpacing">页签栏里相邻芯片的间距。</param>
    /// <param name="contentSpacing">页签栏与内容区之间的间距。</param>
    public TabControl(float railSpacing = DefaultRailSpacing, float contentSpacing = DefaultContentSpacing)
    {
        this.contentSpacing = contentSpacing;
        this.rail = new Stack(Stack.Direction.Horizontal, railSpacing);
        this.Add(this.rail);
    }

    /// <summary>
    /// 追加一个页签：在页签栏末尾加一颗芯片，注册其内容元素（内容不立即挂树，选中时才挂入）。
    /// 第一个加入的页签自动被选中。
    /// </summary>
    /// <param name="title">页签标题（芯片文字）。</param>
    /// <param name="content">该页签的内容元素（每个页签一份，独立持有）。</param>
    /// <returns>新页签的芯片（可继续设 TooltipText 等，返回 Button 以保持可访问性一致）。</returns>
    public Button AddTab(string title, Element content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var index = this.contents.Count;
        var chip = new TabChip(title)
        {
            OnClick = () => this.Select(index)
        };
        this.chips.Add(chip);
        this.contents.Add(content);
        this.rail.Add(chip);

        if (this.SelectedIndex < 0)
        {
            this.Select(0);
        }

        return chip;
    }

    /// <summary>
    /// 选中指定页签：把上一个页签的内容从内容区移除、把新页签内容挂入（结构版本变化 → 焦点图重建），
    /// 若新内容实现 <see cref="IResettable" /> 则 <see cref="IResettable.Reset" />（切到分页内容回到第一页）。
    /// 已在目标页签（或索引越界）时不动作。
    /// </summary>
    /// <param name="index">要选中的页签索引。</param>
    /// <returns>选中发生了实际变化时为 true。</returns>
    public bool Select(int index)
    {
        if (index < 0 || index >= this.contents.Count || index == this.SelectedIndex)
        {
            return false;
        }

        if (this.SelectedIndex >= 0 && this.SelectedContent is { Parent: not null } oldContent)
        {
            this.Remove(oldContent);
        }

        this.SelectedIndex = index;

        for (var i = 0; i < this.chips.Count; i++)
        {
            this.chips[i].Selected = i == index;
        }

        var content = this.contents[index];
        this.Add(content);

        if (content is IResettable resettable)
        {
            resettable.Reset();
        }

        this.MarkDirty();

        return true;
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        if (this.contents.Count == 0)
        {
            return Vector2.Zero;
        }

        this.rail.Measure(new Vector2(available.X, available.Y));
        var railHeight = this.rail.DesiredSize.Y;

        var content = this.SelectedContent;
        var contentWidth = 0f;
        var contentHeight = 0f;

        if (content is not null)
        {
            var availableContentHeight = Math.Max(0f, available.Y - railHeight - this.contentSpacing);
            content.Measure(new Vector2(available.X, availableContentHeight));
            contentWidth = content.DesiredSize.X;
            contentHeight = content.DesiredSize.Y;
        }

        var width = Math.Min(available.X, Math.Max(this.rail.DesiredSize.X, contentWidth));
        var height = Math.Min(available.Y, railHeight + this.contentSpacing + contentHeight);

        return new Vector2(width, height);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        if (this.contents.Count == 0)
        {
            return;
        }

        // 页签栏在顶部（横向满宽排布），内容区在下方占满剩余区域
        var railHeight = (int)this.rail.DesiredSize.Y;
        this.rail.Arrange(new Rectangle(final.X, final.Y, final.Width, railHeight));

        if (this.SelectedContent is { } content)
        {
            var contentY = final.Y + railHeight + (int)this.contentSpacing;
            var contentHeight = Math.Max(0, final.Y + final.Height - contentY);
            content.Arrange(new Rectangle(final.X, contentY, final.Width, contentHeight));
        }
    }
}
