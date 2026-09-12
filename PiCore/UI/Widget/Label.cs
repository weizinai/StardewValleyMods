using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.PiCore.UI.Widget;

/// <summary>
/// 文本标签：测量与绘制用同一游戏语言 SpriteFont（默认 <see cref="Theme.SmallFont" />），保证测得宽 = 绘制宽
/// （无文字阴影，扁平观感）。支持折行（<see cref="MaxWidth" />）：文本自然宽不超有效折行宽时
/// 按自然宽单行测量绘制；超出时按 <see cref="TextWrap" /> 的折行规则折成多行（CJK 逐字、拉丁按词、混合词界退让、
/// 收尾标点粘行、显式 \n 硬断点），多行高 = 行数 × 行高，后续元素被推到文本之下、不重叠。
/// 有效折行宽 = <see cref="MaxWidth" />（显式设置）或布局给到的可用宽（未设置时），因此放在窄容器里的标签
/// 也会自动折行而不是溢出。注意：中文无豆腐块的前提是游戏运行在中文语言（字体是游戏语言绑定的），
/// 本组件只负责用加载的游戏字体测量/绘制，不为字体缺失的脚本负责。
/// </summary>
public class Label : Element
{
    private readonly bool center;
    private string text;
    private SpriteFont? font;
    private float? maxWidth;
    private IReadOnlyList<string>? wrappedLines;
    private float lineHeight;

    /// <summary>构造标签。</summary>
    /// <param name="text">文本内容。</param>
    /// <param name="center">是否在本元素宽度内水平居中。</param>
    public Label(string text, bool center = false)
    {
        this.text = text;
        this.center = center;
    }

    /// <summary>文本内容（改动后标脏重排）。</summary>
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

    /// <summary>文字颜色（默认 <see cref="Theme.TextColor" />）。</summary>
    public Color Color { get; set; } = Theme.TextColor;

    /// <summary>字体（null = <see cref="Theme.SmallFont" />，测量/绘制同源）。</summary>
    public SpriteFont? Font
    {
        get => this.font;
        set
        {
            this.font = value;
            this.MarkDirty();
        }
    }

    /// <summary>最大折行宽（像素）。null = 按布局给到的可用宽折行；文本自然宽不超有效折行宽时仍为单行。</summary>
    public float? MaxWidth
    {
        get => this.maxWidth;
        set
        {
            this.maxWidth = value;
            this.wrappedLines = null;
            this.MarkDirty();
        }
    }

    /// <summary>当前折出的各行（measure 后有效；单行/空文本时为 null）。</summary>
    public IReadOnlyList<string>? WrappedLines => this.wrappedLines;

    private SpriteFont ResolvedFont => this.font ?? Theme.SmallFont;

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        if (string.IsNullOrEmpty(this.text))
        {
            this.wrappedLines = null;

            return Vector2.Zero;
        }

        var font = this.ResolvedFont;
        this.lineHeight = font.LineSpacing;
        var naturalWidth = font.MeasureString(this.text).X;
        var effective = Math.Max(1f, this.maxWidth ?? available.X);

        // 单行：自然宽不超有效折行宽 → 按自然宽单行（期望宽 = 自然宽，高 = 行高）
        if (naturalWidth <= effective)
        {
            this.wrappedLines = null;

            return new Vector2(naturalWidth, this.lineHeight);
        }

        // 多行：按 TextWrap 折行规则折行，期望宽 = 有效折行宽，高 = 行数 × 行高（后续元素被推到下方、不重叠）
        this.wrappedLines = TextWrap.Wrap(font, this.text, effective);

        return new Vector2(effective, this.wrappedLines.Count * this.lineHeight);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 就地布置：文字绘制于本元素左上角（居中时以宽度中线对齐）
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        if (string.IsNullOrEmpty(this.text))
        {
            return;
        }

        var font = this.ResolvedFont;

        if (this.wrappedLines is null)
        {
            var size = font.MeasureString(this.text);
            var x = this.center ? this.Bounds.Center.X - size.X / 2f : this.Bounds.X;
            Theme.DrawText(batch, this.text, new Vector2(x, this.Bounds.Y), this.Color, font);

            return;
        }

        // y 是行高累加（float），不能用 var（Rectangle.Y 是 int，会截断行高）
        float y = this.Bounds.Y;

        foreach (var line in this.wrappedLines)
        {
            var x = this.center ? this.Bounds.Center.X - font.MeasureString(line).X / 2f : this.Bounds.X;
            Theme.DrawText(batch, line, new Vector2(x, y), this.Color, font);
            y += this.lineHeight;
        }
    }
}
