using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.PiCore.UI.World;

/// <summary>
/// 小号世界锚定「面板 + 文本行」盒（form C 逃生口，镜像 BetterCabin Box / 验证原型 WorldPanelDemo）：
/// 承载若干文本行（标题用 <see cref="Theme.DialogueFont" />、正文用 <see cref="Theme.SmallFont" />，可逐行配色），
/// 用游戏语言 SpriteFont 测量（测得宽 = 绘制宽，中文无豆腐块依赖中文游戏语言），画成扁平九宫格面板 +
/// 扁平文字。本类只负责「内容」：测量自身尺寸并在给定屏幕矩形内绘制；世界→屏幕变换与视口裁剪由
/// 世界锚定宿主承担，也可在任意 RenderedWorld 处理器里直接 Measure + Draw 自绘。
/// </summary>
public sealed class TilePanel : IAnchoredContent
{
    /// <summary>一行文本的不可变描述（文本 + 可选字体/颜色）。</summary>
    private readonly struct Line
    {
        public Line(string text, SpriteFont? font, Color? color)
        {
            this.Text = text;
            this.Font = font;
            this.Color = color;
        }

        public string Text { get; }

        public SpriteFont? Font { get; }

        public Color? Color { get; }
    }

    /// <summary>面板内容相对边框四周的默认内缩（像素）。</summary>
    private const float DefaultPadding = 16f;

    /// <summary>相邻文本行之间的默认垂直间距（像素）。</summary>
    private const float DefaultLineSpacing = 4f;

    private readonly List<Line> lines = new();

    /// <summary>构造面板。</summary>
    public TilePanel() { }

    /// <summary>面板内容相对边框四周的内缩（像素）。</summary>
    public float Padding { get; set; } = DefaultPadding;

    /// <summary>相邻文本行之间的垂直间距（像素）。</summary>
    public float LineSpacing { get; set; } = DefaultLineSpacing;

    /// <summary>追加一行标题（用 <see cref="Theme.DialogueFont" />，默认文字色）。</summary>
    /// <param name="text">文本。</param>
    /// <returns>本面板（链式调用）。</returns>
    public TilePanel AddTitle(string text)
    {
        return this.AddLine(text, Theme.DialogueFont);
    }

    /// <summary>追加一行正文（用 <see cref="Theme.SmallFont" />，默认文字色）。</summary>
    /// <param name="text">文本。</param>
    /// <returns>本面板（链式调用）。</returns>
    public TilePanel AddText(string text)
    {
        return this.AddLine(text);
    }

    /// <summary>追加一行文本（可选字体/颜色；null 用各自默认）。</summary>
    /// <param name="text">文本。</param>
    /// <param name="font">字体（null = <see cref="Theme.SmallFont" />）。</param>
    /// <param name="color">文字颜色（null = <see cref="Theme.TextColor" />）。</param>
    /// <returns>本面板（链式调用）。</returns>
    public TilePanel AddLine(string text, SpriteFont? font = null, Color? color = null)
    {
        this.lines.Add(new Line(text, font, color));

        return this;
    }

    /// <inheritdoc />
    public Vector2 Measure()
    {
        var maxWidth = 0f;
        var totalHeight = 0f;

        foreach (var line in this.lines)
        {
            var font = line.Font ?? Theme.SmallFont;
            var size = font.MeasureString(line.Text);
            maxWidth = Math.Max(maxWidth, size.X);
            totalHeight += font.LineSpacing;
        }

        if (this.lines.Count > 1)
        {
            totalHeight += this.LineSpacing * (this.lines.Count - 1);
        }

        return new Vector2(maxWidth + 2f * this.Padding, totalHeight + 2f * this.Padding);
    }

    /// <inheritdoc />
    public void Draw(SpriteBatch batch, Rectangle bounds)
    {
        if (this.lines.Count == 0)
        {
            return;
        }

        Theme.DrawPanel(batch, bounds.X, bounds.Y, bounds.Width, bounds.Height);

        // y 是行高累加（float），不能用 var（Rectangle.Y 是 int，会截断行高）
        var y = bounds.Y + this.Padding;

        foreach (var line in this.lines)
        {
            var font = line.Font ?? Theme.SmallFont;
            Theme.DrawText(batch, line.Text, new Vector2(bounds.X + this.Padding, y), line.Color, font);
            y += font.LineSpacing + this.LineSpacing;
        }
    }
}
