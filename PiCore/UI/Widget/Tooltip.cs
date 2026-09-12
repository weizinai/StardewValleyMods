using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 提示框（Tooltip）：悬停在视图树之上的扁平九宫格小面板 + 文本，由宿主（<see cref="Host.MenuHost" />）
/// 在需要时排版并绘制，**不加入布局树**（浮动层，宿主持有单个实例反复使用）。
/// 两种摆放行为（与宿主配合）：鼠标驱动时跟随光标、鼠标离开即消失（不残留钉住）；手柄驱动时
/// 锚定到当前获焦项旁边（绝不压住获焦项或其焦点环）并跟随焦点移动。
/// 正文折行：提示框正文按 <see cref="TextWrap" /> 的同一契约折行——内容超 <see cref="BodyMaxWidth" />
/// 时折成多行、面板包住折后的文本（宽 = 最宽行 + 内边距，高 = 行数 × 行高 + 内边距）。中文无豆腐块的前提
/// 是游戏运行在中文语言（字体是游戏语言绑定的），本组件只负责用加载的游戏字体测量/绘制。
/// </summary>
public class Tooltip : Element
{
    /// <summary>文本左右内边距（面板最小宽度 = 文本宽 + 2×该值）。</summary>
    private const float HorizontalPadding = 20f;

    /// <summary>文本上下内边距（面板最小高度 = 文本高 + 2×该值）。</summary>
    private const float VerticalPadding = 12f;

    /// <summary>正文最大折行内宽（像素，超过即折行；面板宽度 = 最宽行 + 内边距，不超该值 + 内边距）。</summary>
    private const float BodyMaxWidth = 360f;

    private string text;
    private IReadOnlyList<string>? wrappedLines;
    private float lineHeight;

    /// <summary>构造提示框（默认无文本；宿主显示前设置 <see cref="Text" />）。</summary>
    public Tooltip()
    {
        this.text = string.Empty;
    }

    /// <summary>提示框文本（改动后标脏；宿主显示前重测重排）。</summary>
    public string Text
    {
        get => this.text;
        set
        {
            this.text = value;
            this.wrappedLines = null;
            this.MarkDirty();
        }
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        if (string.IsNullOrEmpty(this.text))
        {
            return Vector2.Zero;
        }

        var font = Theme.SmallFont;
        this.lineHeight = font.LineSpacing;
        var innerWidth = Math.Max(1f, Math.Min(BodyMaxWidth, available.X - HorizontalPadding * 2f));
        this.wrappedLines = TextWrap.Wrap(font, this.text, innerWidth);

        var width = this.wrappedLines.Select(line => font.MeasureString(line).X).Prepend(0f).Max();

        var boxWidth = Math.Min(width + HorizontalPadding * 2f, available.X);
        var boxHeight = Math.Min(this.wrappedLines.Count * this.lineHeight + VerticalPadding * 2f, available.Y);

        return new Vector2(boxWidth, boxHeight);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 就地布置：面板与文本绘制于本元素矩形内
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        if (string.IsNullOrEmpty(this.text))
        {
            return;
        }

        Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);

        // 未先测量就直接绘制时退化为单行（宿主流程总是先 Measure 再 Draw，此处仅为健壮性兜底）
        if (this.wrappedLines is null)
        {
            Theme.DrawText(batch, this.text, new Vector2(this.Bounds.X + HorizontalPadding, this.Bounds.Y + VerticalPadding), Theme.TextColor);

            return;
        }

        var y = this.Bounds.Y + VerticalPadding;

        foreach (var line in this.wrappedLines)
        {
            Theme.DrawText(batch, line, new Vector2(this.Bounds.X + HorizontalPadding, y), Theme.TextColor);
            y += this.lineHeight;
        }
    }
}
