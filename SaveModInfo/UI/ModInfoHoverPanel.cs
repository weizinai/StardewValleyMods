using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;
using weizinai.StardewValleyMod.SaveModInfo.Record;

namespace weizinai.StardewValleyMod.SaveModInfo.UI;

/// <summary>
/// 悬停概览面板：鼠标停在图标上时出现的扁平小面板，给出「已移除 / 新增 / 已更新」各有多少条，
/// 外加一句「点击图标查看详情」（没有记录时改为说明无法对比），跟随光标、超宽折行、永不画出视口。
/// </summary>
/// <remarks>
///     <para>
///     <b>不进布局树</b>（与框架自带提示框同一设计）：根视图持有它一个实例，在绘制期按当前光标现测现摆现画，
///     不参与顺序堆叠，因此「跟随光标」的位移不牵动任何兄弟元素的布局、也不需要任何标脏。内容只在差异结果换了
///     一个实例时才重建，光标移动不会碰到它。
///     </para>
///     <para>
///     折行与「不溢出视口」都走框架契约：正文按 <see cref="TextWrap" />（CJK 逐字 / 拉丁按词，与 <see cref="Label" />
///     同源）在 <see cref="MaxInnerWidth" /> 内折行，折出的行缓存在测量结果里供绘制复用（与框架提示框同一做法，
///     避免绘制期与测量期折出不一样的行）；面板尺寸取折后文本的实测包围盒，光标贴近视口边缘时整块翻侧。
///     </para>
/// </remarks>
internal sealed class ModInfoHoverPanel : Element
{
    /// <summary>面板最小内宽（内容再短也不小于它，避免一行小面板读起来挤）。</summary>
    private const float MinInnerWidth = 220f;

    /// <summary>面板最大内宽（超宽即折行）。与框架提示框的正文最大宽同值。</summary>
    private const float MaxInnerWidth = 360f;

    /// <summary>文本四周内边距。</summary>
    private const float Padding = 16f;

    /// <summary>面板与光标之间的间隙（避免面板压住光标）。</summary>
    private const int CursorGap = 24;

    /// <summary>本帧的内容行（三类计数摘要 + 查看详情提示；没有记录时改为说明无法对比）；由 <see cref="Sync" /> 现取语言文案写入。</summary>
    private IReadOnlyList<string> lines = Array.Empty<string>();

    /// <summary>折好的行（测量后有效），绘制时直接复用。</summary>
    private IReadOnlyList<string>? wrappedLines;

    /// <summary>上次写入的差异结果，用于判断内容有没有真的变化（避免每帧重取文案）。</summary>
    private ModDiff? shownDiff;

    /// <summary>按差异结果同步内容（只有换到另一个差异结果时才重取文案）。</summary>
    /// <param name="diff">该存档的差异结果；null = 本帧不显示概览（没有悬停任何图块，或模态窗口正打开）。</param>
    public void Sync(ModDiff? diff)
    {
        if (ReferenceEquals(this.shownDiff, diff))
        {
            return;
        }

        this.shownDiff = diff;
        this.lines = diff is null ? Array.Empty<string>() : this.BuildLines(diff);

        // 折行结果随文案失效：置空让下一次测量重折（本面板不进布局树，测量由根视图在绘制期调用）
        this.wrappedLines = null;
    }

    /// <summary>把面板摆到光标旁并夹进视口（右侧放不下翻到光标左侧，下方放不下翻到光标上方）。</summary>
    /// <param name="cursorX">光标 X（UI 坐标）。</param>
    /// <param name="cursorY">光标 Y（UI 坐标）。</param>
    /// <returns>面板左上角（已夹进视口，保证整块可见）。</returns>
    public Vector2 Place(int cursorX, int cursorY)
    {
        var size = this.DesiredSize;
        var viewport = new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height);
        var x = cursorX + CursorGap;
        var y = cursorY + CursorGap;

        if (x + size.X > viewport.Right)
        {
            x = cursorX - CursorGap - (int)size.X;
        }

        if (y + size.Y > viewport.Bottom)
        {
            y = cursorY - CursorGap - (int)size.Y;
        }

        x = Math.Clamp(x, viewport.X, Math.Max(viewport.X, viewport.Right - (int)size.X));
        y = Math.Clamp(y, viewport.Y, Math.Max(viewport.Y, viewport.Bottom - (int)size.Y));

        return new Vector2(x, y);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        var font = Theme.SmallFont;
        var innerWidth = Math.Max(1f, Math.Min(MaxInnerWidth, available.X - Padding * 2f));

        this.wrappedLines = TextWrap.Wrap(font, string.Join("\n", this.lines), innerWidth);

        var width = Math.Max(MinInnerWidth, this.wrappedLines.Select(line => font.MeasureString(line).X).Prepend(0f).Max());

        return new Vector2(
            Math.Min(width + Padding * 2f, available.X),
            this.wrappedLines.Count * font.LineSpacing + Padding * 2f);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 就地布置：文本绘制于本元素矩形内
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);

        // 未先测量就直接绘制时退化为空面板（根视图的流程总是先测量再布置，此处仅为健壮性兜底）
        if (this.wrappedLines is null)
        {
            return;
        }

        var y = this.Bounds.Y + Padding;

        foreach (var line in this.wrappedLines)
        {
            Theme.DrawText(batch, line, new Vector2(this.Bounds.X + Padding, y), Theme.TextColor);
            y += Theme.SmallFont.LineSpacing;
        }
    }

    /// <summary>按差异结果拼出概览文本行：三类各多少条 + 一句查看详情的提示。</summary>
    /// <param name="diff">差异结果。</param>
    /// <returns>文本行：没有记录时是「无法对比」的说明，否则是三类计数再加一句提示。</returns>
    private IReadOnlyList<string> BuildLines(ModDiff diff)
    {
        // 从未记录过的存档也要显示图标（否则玩家会把「没记录」误当成「一切一致」），概览里说明原因
        if (!diff.HasRecord)
        {
            return new[] { I18n.UI_ModInfo_NoRecord(), I18n.UI_ModInfo_DetailsHint() };
        }

        // 三类计数**全列**（含 0 的那类）：概览要回答的就是「三类里各有多少」，只列非零项会让面板忽长忽短，
        // 也让「零」这件事看不见——而「一个模组都没少，只是加了两个」正是改造前那句 tooltip 说不清的场景。
        // 明细留给窗口，概览只给数量和入口
        return new[]
        {
            I18n.UI_ModInfo_RemovedCount(diff.Removed.Count),
            I18n.UI_ModInfo_AddedCount(diff.Added.Count),
            I18n.UI_ModInfo_UpdatedCount(diff.Updated.Count),
            I18n.UI_ModInfo_DetailsHint()
        };
    }
}
