using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 九宫格外框容器：用 <see cref="Theme.DrawPanel" /> 画
/// 一块标准菜单盒九宫格外框，内容在其内侧按构造时的 padding 内缩排布。默认观感为 SDV 原生形状 +
/// 扁平（无投影），替代消费模组手写的 drawTextureBox 外框 + 手算内偏移。
/// 面板承载**单一内容**：内容应经 <see cref="SetContent" /> 挂入（先清空再加）。请勿直接向面板多次
/// <see cref="Element.Add" /> 子级——measure 取最后可见子级的尺寸、arrange 把所有子级排进同一内缩矩形，
/// 多个子级会彼此重叠。
/// </summary>
public class PanelFrame : Element
{
    private readonly float padding;

    /// <summary>构造面板。</summary>
    /// <param name="padding">内容相对外框四周的内缩量（像素）。</param>
    public PanelFrame(float padding = 36f)
    {
        this.padding = padding;
    }

    /// <summary>把唯一内容加进来（先清空再加，面板只承载一份内容）。</summary>
    /// <param name="content">面板内容。</param>
    /// <exception cref="ArgumentNullException"><paramref name="content" /> 为 null。</exception>
    public void SetContent(Element content)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        this.Clear();
        this.Add(content);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        Vector2 innerAvailable = new(
            Math.Max(0f, available.X - 2f * this.padding),
            Math.Max(0f, available.Y - 2f * this.padding));
        var inner = Vector2.Zero;

        foreach (var child in this.Children)
        {
            if (!child.Visible)
            {
                continue;
            }

            inner = child.Measure(innerAvailable);
        }

        return new Vector2(inner.X + 2f * this.padding, inner.Y + 2f * this.padding);
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

            child.Arrange(new Rectangle(
                final.X + (int)this.padding,
                final.Y + (int)this.padding,
                (int)Math.Max(0f, final.Width - 2f * this.padding),
                (int)Math.Max(0f, final.Height - 2f * this.padding)));
        }
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);
    }
}
