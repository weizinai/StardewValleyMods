using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 分页容器（Pager，票 06 高阶控件，蒸馏自 Demo A 手搓的「内容 + 上页/下页 + 页码」样板）：
/// 内容区在同一块区域内**只挂当前页**（一页一个 <see cref="Element" />，通常是容纳一页网格内容的
/// <see cref="Grid" />），底部居中翻页条（<see cref="PreviousButton" /> + <see cref="PageLabel" /> +
/// <see cref="NextButton" />）逐页切换，页码文本随翻页自动更新；首/末页把对应的翻页按钮隐藏
/// （隐藏 = 不布局不绘制不可获焦，焦点可自然落到其余项）。换页即换内容子级：Add/Remove 触发
/// <see cref="Element.TreeStructureVersion" /> 变化 → 宿主自动重建焦点图（保留仍存在的焦点）。
/// 页面内容不限定形态：消费方用 <see cref="AddPage" /> 逐页加入内容（每页一块网格等），内容尺寸在
/// measure/arrange 时按“内容在上、翻页条在下”竖向排布。切换页签等需要“回到第一页”的场合可调
/// <see cref="ResetToFirstPage" />（TabControl 在激活内容为分页器时自动调用）。
/// </summary>
public class Pager : Element, IResettable
{
    /// <summary>内容区与底部翻页条之间的默认间距。</summary>
    private const float DefaultSpacing = 8f;

    /// <summary>翻页条内部（上一页按钮 / 页码 / 下一页按钮）的默认间距。</summary>
    private const float BarSpacing = 12f;

    private readonly Stack bar;
    private readonly List<Element> pages = new();
    private readonly Button previousButton;
    private readonly Label pageLabel;
    private readonly Button nextButton;
    private readonly float spacing;

    /// <summary>构造分页容器（尚无任何页面；内容在上、翻页条在下居中）。</summary>
    /// <param name="spacing">内容区与底部翻页条之间的间距。</param>
    public Pager(float spacing = DefaultSpacing)
    {
        this.spacing = spacing;
        this.previousButton = new Button(string.Empty);
        this.nextButton = new Button(string.Empty);
        this.pageLabel = new Label(string.Empty, center: true);

        // 翻页条：上页 / 页码 / 下页；按钮文字由消费方按需设置（框架不含任何语言文案）。
        this.previousButton.OnClick = this.PreviousPage;
        this.nextButton.OnClick = this.NextPage;
        this.bar = new Stack(Stack.Direction.Horizontal, BarSpacing);
        this.bar.Add(this.previousButton);
        this.bar.Add(this.pageLabel);
        this.bar.Add(this.nextButton);
        this.Add(this.bar);

        this.RefreshBar(); // 无页面 → 翻页条默认隐藏
    }

    /// <summary>
    /// 当前页码文本格式器（参数：当前页 1 基、总页数；返回要显示的页码文本）。
    /// 为 null 时默认显示 “当前页/总页数”（如 1/2）。消费方可替换成本地化/带统计信息的文案。
    /// </summary>
    public Func<int, int, string>? PageLabelFormatter { get; set; }

    /// <summary>上一页按钮（点击已接好；消费方可设置其文字/样式）。</summary>
    public Button PreviousButton => this.previousButton;

    /// <summary>下一页按钮（点击已接好；消费方可设置其文字/样式）。</summary>
    public Button NextButton => this.nextButton;

    /// <summary>页码标签（分页器自动更新其文本；消费方可调整颜色/字体等）。</summary>
    public Label PageLabel => this.pageLabel;

    /// <summary>当前页数。</summary>
    public int PageCount => this.pages.Count;

    /// <summary>当前页索引（0 基；无页面时为 -1）。</summary>
    public int CurrentPage { get; private set; } = -1;

    /// <summary>当前挂载的页面内容元素（无页面时为 null）。</summary>
    public Element? CurrentContent => this.CurrentPage >= 0 && this.CurrentPage < this.pages.Count ? this.pages[this.CurrentPage] : null;

    /// <summary>追加一页内容并立即显示（若尚未显示任何页）；返回本分页器以支持链式调用。</summary>
    /// <param name="page">该页内容元素（通常是一块 <see cref="Grid" />）。</param>
    /// <returns>本分页器。</returns>
    public Pager AddPage(Element page)
    {
        if (page is null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        this.pages.Add(page);

        if (this.CurrentPage < 0)
        {
            this.GoToPage(0);
        }
        else
        {
            this.RefreshBar();
            this.MarkDirty();
        }

        return this;
    }

    /// <summary>跳到指定页（0 基，越界自动夹紧）；页有实际变化时换挂内容并返回 true。</summary>
    /// <param name="index">目标页索引。</param>
    /// <returns>页发生实际变化时为 true（未变化 / 无页面时为 false）。</returns>
    public bool GoToPage(int index)
    {
        if (this.pages.Count == 0)
        {
            return false;
        }

        var target = Math.Clamp(index, 0, this.pages.Count - 1);

        if (target == this.CurrentPage)
        {
            return false;
        }

        if (this.CurrentPage >= 0)
        {
            this.Remove(this.pages[this.CurrentPage]);
        }

        this.CurrentPage = target;
        this.Add(this.pages[target]);
        this.RefreshBar();

        return true;
    }

    /// <summary>上一页（已在首页则不动作）。</summary>
    public void PreviousPage()
    {
        this.GoToPage(this.CurrentPage - 1);
    }

    /// <summary>下一页（已在末页则不动作）。</summary>
    public void NextPage()
    {
        this.GoToPage(this.CurrentPage + 1);
    }

    /// <summary>回到第一页（页签切换后“回到第一页”语义用；已在第一页则不动作）。</summary>
    public void ResetToFirstPage()
    {
        this.GoToPage(0);
    }

    /// <summary>实现 <see cref="IResettable" />：回到第一页（页签激活时的“回到初始态”契约）。</summary>
    public void Reset()
    {
        this.ResetToFirstPage();
    }

    /// <summary>刷新翻页条：首/末页隐藏对应按钮、更新页码文本、无页面时隐藏整条翻页条。</summary>
    private void RefreshBar()
    {
        var hasPages = this.pages.Count > 0;
        this.bar.Visible = hasPages;

        if (!hasPages)
        {
            return;
        }

        var lastIndex = this.pages.Count - 1;
        this.previousButton.Visible = this.CurrentPage > 0;
        this.nextButton.Visible = this.CurrentPage < lastIndex;
        this.pageLabel.Text = this.PageLabelFormatter is { } formatter
            ? formatter(this.CurrentPage + 1, this.pages.Count)
            : $"{this.CurrentPage + 1}/{this.pages.Count}";
        this.MarkDirty();
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        if (this.pages.Count == 0)
        {
            return Vector2.Zero;
        }

        this.bar.Measure(new Vector2(available.X, available.Y));
        var barWidth = this.bar.DesiredSize.X;
        var barHeight = this.bar.DesiredSize.Y;

        var page = this.CurrentContent;
        var contentWidth = 0f;
        var contentHeight = 0f;

        if (page is not null)
        {
            var availableContentHeight = Math.Max(0f, available.Y - barHeight - this.spacing);
            page.Measure(new Vector2(available.X, availableContentHeight));
            contentWidth = page.DesiredSize.X;
            contentHeight = page.DesiredSize.Y;
        }

        var width = Math.Min(available.X, Math.Max(barWidth, contentWidth));
        var height = Math.Min(available.Y, barHeight + (page is null ? 0f : this.spacing + contentHeight));

        return new Vector2(width, height);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        var page = this.CurrentContent;
        var barHeight = this.bar.DesiredSize.Y;
        var contentHeight = page is null
            ? 0f
            : Math.Min(page.DesiredSize.Y, Math.Max(0f, final.Height - barHeight - this.spacing));

        // 内容在上（占满可用宽度，让网格自行整块居中），翻页条在内容下方水平居中
        page?.Arrange(new Rectangle(final.X, final.Y, final.Width, (int)contentHeight));

        var barY = final.Y + contentHeight + this.spacing;
        var barWidth = Math.Min(this.bar.DesiredSize.X, final.Width);
        var barX = final.X + (final.Width - barWidth) / 2f;
        this.bar.Arrange(new Rectangle((int)barX, (int)barY, (int)barWidth, (int)barHeight));
    }
}
