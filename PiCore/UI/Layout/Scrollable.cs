using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.PiCore.UI.Layout;

/// <summary>
/// 可滚动视口容器（列表容器）：固定尺寸的剪裁区域，纵向堆叠子级。滚动偏移在
/// <see cref="ArrangeOverride" /> 时折算进子级 Bounds（子级得到真实屏幕坐标；滚动后越出视口的
/// 子级仍保有布局坐标，供几何导航与命中测试使用）。绘制时用 scissor 把内容裁剪在内容区内，不溢出。
/// 内容区相对视口边框内缩 <see cref="ContentInset" />（九宫格面板边框厚度 + 留白，语义同
/// <see cref="Host.MenuHost" /> 的 ContentInset）：子级排布在边框内侧；绘制与命中测试也裁到该内容区，滚动时文字/按钮不压边框。
/// 滚动由 <see cref="ScrollTo" /> / <see cref="ScrollBy" /> 驱动（鼠标滚轮与手柄右摇杆由
/// <see cref="Host.MenuHost" /> 转发）。滚动变化即标脏整条父链：任何读取 Bounds 做屏幕定位的代码
/// （如焦点光标落点）先经 <see cref="LayoutRunner.UpdateIfDirty(Element, Rectangle)" /> 冲刷布局，保证同帧读到滚动后的新坐标，
/// 不读到滚动前旧值。
/// </summary>
public class Scrollable : Element
{
    /// <summary>相邻子级默认间隔。</summary>
    private const float DefaultSpacing = 8f;

    /// <summary>
    /// 内容区相对视口边框的内缩（九宫格面板边框厚度 + 留白；语义同 <see cref="Host.MenuHost" /> 的 ContentInset），
    /// 子级排布在边框内侧、文字/按钮不压边框。
    /// </summary>
    private const float ContentInset = 24f;

    /// <summary>scissor 裁剪用的光栅化状态（复用单例，避免每帧新建对象）。</summary>
    private static readonly RasterizerState ScissorRasterizer = new() { ScissorTestEnable = true };

    private readonly float viewportWidth;
    private readonly float viewportHeight;
    private readonly float spacing;

    private float scrollOffset;
    private float contentHeight;

    /// <summary>
    /// 构造可滚动视口。高度固定（内容超高才滚动）；宽度 <c>0</c> 表示不约束、填满可用宽度，
    /// 非 0 时是期望视口宽度（若被父容器拉伸则取实际布置宽度，绘制与命中以实际布置矩形为准）。
    /// </summary>
    /// <param name="width">期望视口宽度（0 = 填满可用宽度）。</param>
    /// <param name="height">期望视口高度。</param>
    /// <param name="spacing">相邻子级间隔。</param>
    public Scrollable(float width, float height, float spacing = DefaultSpacing)
    {
        this.viewportWidth = width;
        this.viewportHeight = height;
        this.spacing = spacing;
    }

    /// <summary>当前滚动偏移（内容相对视口顶部下移的像素量，≥0）。</summary>
    public float ScrollOffset => this.scrollOffset;

    /// <summary>内容区矩形（视口边框内缩 <see cref="ContentInset" /> 后，arrange 后有效）：子级在此区域内排布/测量/滚入；绘制 scissor 与命中测试也裁到此矩形，滚动时内容不压边框。</summary>
    internal Rectangle InnerViewport
    {
        get
        {
            var inset = (int)ContentInset;

            return new Rectangle(
                this.Bounds.X + inset,
                this.Bounds.Y + inset,
                Math.Max(0, this.Bounds.Width - inset * 2),
                Math.Max(0, this.Bounds.Height - inset * 2));
        }
    }

    /// <summary>可滚动的最大偏移（内容高 − 内容区高；内容不满内容区时为 0）。</summary>
    public float MaxScrollOffset => Math.Max(0f, this.contentHeight - this.InnerViewport.Height);

    /// <summary>是否把视口当作列表盒子绘制（九宫格背景，默认 true；内容被裁剪在盒子内，直观呈现不溢出）。</summary>
    public bool DrawPanel { get; set; } = true;

    /// <summary>滚动到指定偏移（裁剪到合法范围）；偏移实际变化时标脏并返回 true。</summary>
    /// <param name="offset">目标偏移。</param>
    /// <returns>偏移发生实际变化时为 true。</returns>
    public bool ScrollTo(float offset)
    {
        var clamped = Math.Clamp(offset, 0f, this.MaxScrollOffset);

        if (Math.Abs(clamped - this.scrollOffset) < 0.5f)
        {
            return false;
        }

        this.scrollOffset = clamped;
        this.MarkDirty();

        return true;
    }

    /// <summary>按增量滚动（正值向下、负值向上）；返回是否实际变化。</summary>
    /// <param name="delta">滚动增量。</param>
    /// <returns>偏移发生实际变化时为 true。</returns>
    public bool ScrollBy(float delta)
    {
        return this.ScrollTo(this.scrollOffset + delta);
    }

    /// <summary>把 <paramref name="item" /> 滚入视口（焦点自动滚入用）：越出顶部时上滚对齐内容区顶边，越出底部时下滚对齐内容区底边。</summary>
    /// <param name="item">要滚入视口的元素。</param>
    internal void EnsureVisible(Element item)
    {
        var viewport = this.InnerViewport;

        if (item.Bounds.Top < viewport.Top)
        {
            this.ScrollBy(item.Bounds.Top - viewport.Top);
        }
        else if (item.Bounds.Bottom > viewport.Bottom)
        {
            this.ScrollBy(item.Bounds.Bottom - viewport.Bottom);
        }
    }

    /// <summary>最近的滚动列表容器祖先（无则 null）。焦点导航的“同列表”与右摇杆滚动目标都指最近者。</summary>
    /// <param name="item">元素。</param>
    internal static Scrollable? FindAncestor(Element item)
    {
        for (var node = item.Parent; node is not null; node = node.Parent)
        {
            if (node is Scrollable scrollable)
            {
                return scrollable;
            }
        }

        return null;
    }

    /// <summary><paramref name="item" /> 是否为本滚动容器的后代（含多层嵌套）。</summary>
    /// <param name="item">元素。</param>
    internal bool IsAncestorOf(Element item)
    {
        for (var node = item.Parent; node is not null; node = node.Parent)
        {
            if (ReferenceEquals(node, this))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        // 宽度 0 = 不约束，填满可用宽度；非 0 取期望宽度（封顶于可用宽度）。高度取期望（封顶于可用高度）。
        var viewportW = this.viewportWidth > 0f ? Math.Min(this.viewportWidth, available.X) : available.X;
        var viewportH = Math.Min(this.viewportHeight, available.Y);

        // 内容区宽 = 视口宽 − 两侧内缩：子级在边框内侧测量（窄容器里的长文本按内宽折行，不压边框）。
        var innerW = Math.Max(1f, viewportW - ContentInset * 2f);

        // 子级沿主轴按“不限高”测量（不被视口高度压扁），内容取自然高度总和——视口裁剪只发生在绘制阶段。
        var content = 0f;
        var maxChildWidth = 0f;
        var visible = 0;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            child.Measure(new Vector2(innerW, float.MaxValue));
            content += child.DesiredSize.Y;
            maxChildWidth = Math.Max(maxChildWidth, child.DesiredSize.X);
            visible++;
        }

        if (visible > 1)
        {
            content += (visible - 1) * this.spacing;
        }

        this.contentHeight = content;

        return new Vector2(viewportW, viewportH);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        var inner = this.InnerViewport;

        // 布置前先把偏移裁到合法范围（以内容区高度为准，布局尺寸变化后可能越界）
        this.scrollOffset = Math.Clamp(this.scrollOffset, 0f, Math.Max(0f, this.contentHeight - inner.Height));

        // 滚动偏移折算进子级 Bounds：内容在内容区内整体上移 scrollOffset，子级得到真实屏幕坐标（不压边框）
        var cursor = inner.Y - this.scrollOffset;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            child.Arrange(new Rectangle(inner.X, (int)Math.Round(cursor), inner.Width, (int)Math.Round(child.DesiredSize.Y)));
            cursor += child.DesiredSize.Y + this.spacing;
        }
    }

    /// <summary>绘制视口背景后，用 scissor 把子级裁剪在内容区内（vanilla QuestLog 同款模式：End → 开 scissor → 画 → 恢复）。</summary>
    /// <inheritdoc />
    public override void Draw(SpriteBatch batch)
    {
        if (!this.Visible)
        {
            return;
        }

        if (this.DrawPanel)
        {
            Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);
        }

        // 裁到内容区（InnerViewport）而不是整块 Bounds：九宫格边框画在 scissor 之外保持完整，
        // 滚动时半出行文字不会画到上下边框上。与当前 ScissorRectangle 相交，天然支持嵌套
        // Scrollable（内层再裁内层）；恢复时用进入前的 RasterizerState，嵌套时能回到外层
        // scissor 状态而不是错误地关闭裁剪。
        var savedScissor = batch.GraphicsDevice.ScissorRectangle;
        var savedRasterizer = batch.GraphicsDevice.RasterizerState;
        batch.End();
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, ScissorRasterizer);
        batch.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(savedScissor, this.InnerViewport);

        foreach (var child in this.Children)
        {
            if (child.Visible)
            {
                child.Draw(batch);
            }
        }

        batch.End();
        batch.GraphicsDevice.ScissorRectangle = savedScissor;
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, savedRasterizer);
    }
}
