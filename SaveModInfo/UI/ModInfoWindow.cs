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
/// 模态窗口：铺满视口的点击拦截层 + 背景压暗 + 居中的面板（标题、记录元信息行、按「已移除 / 新增 / 已更新」
/// 分节的明细列表、关闭按钮）。
/// </summary>
/// <remarks>
///     <para>
///     <b>模态契约</b>：打开期间吞掉所有鼠标左键（含面板的空白处）——靠一层铺满视口、不绘制任何像素的
///     <see cref="BlockerButton" /> 承担。它在树里排在面板**之前**（先绘制、命中优先级低），于是面板上的按钮仍能命中，
///     面板之外的点击落到它身上被消费而窗口背后的存档槽收不到点击。关闭由调用方经构造时传入的动作接走
///     （关闭按钮与 <c>Esc</c> 两条路径都汇到它）。
///     </para>
///     <para>
///     面板宽度定死（模组名长短不会让窗口忽宽忽窄），高度随内容生长并封顶于视口：明细列表超出即在列表内滚动，
///     没有记录时窗口自然收短成「一句说明」的高度。视口放不下时整块夹进视口留边。
///     </para>
///     <para>
///     明细**只挂有内容的分节**：三类里没发生的那几类不占位置，玩家一眼看到的就只有真的变了的东西。
///     窗口只在图标可见的槽位上开得出来（见 <see cref="SaveSlotTile.Sync" />），因此「有记录且三类全空」
///     这种没有内容可列的差异不会到这里——真到了也只会得到一个空的列表盒，不会报错。
///     </para>
///     <para>
///     文案按当前语言现取：模组的 <c>Entry</c> 早于游戏应用已保存的语言（SMAPI 在模组加载后才把翻译切到该语言），
///     构造期取一次会把英文基文案永久留在界面上。内容每次 <see cref="Sync" /> 整体重建（列表回到顶部）。
///     </para>
/// </remarks>
internal sealed class ModInfoWindow : Element
{
    /// <summary>面板宽度（UI 像素）。</summary>
    private const int PanelWidth = 800;

    /// <summary>面板高度（UI 像素）。</summary>
    private const int PanelHeight = 560;

    /// <summary>面板与视口边缘之间的最小留边。</summary>
    private const int ViewportMargin = 32;

    /// <summary>标题行与明细列表之间的间隔。</summary>
    private const float Spacing = 12f;

    /// <summary>明细列表的期望视口高度（面板高度按内容生长并封顶于视口，故实际高度可能被压到更矮）。</summary>
    private const float ListHeight = 380f;

    /// <summary>相邻分节之间的间隔（比节内行距大，让分节的边界看得出来）。</summary>
    private const float SectionSpacing = 20f;

    /// <summary>同一分节内相邻条目行的间隔。</summary>
    private const float ItemSpacing = 4f;

    /// <summary>不绘制任何像素的按钮：只用来吞掉落在面板之外的点击。</summary>
    private sealed class BlockerButton : Button
    {
        /// <summary>构造空文案的拦截按钮。</summary>
        public BlockerButton() : base(string.Empty) { }

        /// <summary>
        /// 本层铺满视口，故**整块不绘制**：按钮的悬停描边画在自身矩形之外一圈、会沿整个屏幕边缘亮一圈金边。
        /// 这一层存在的意义只有命中与消费。
        /// </summary>
        /// <param name="batch">精灵批（未用）。</param>
        protected override void DrawSelf(SpriteBatch batch)
        {
            // 有意留空（理由见本方法摘要）
        }
    }

    /// <summary>标题行：标题占左侧剩余宽度，关闭按钮贴右，两者在同一垂直中线上（框架没有现成的左右分栏容器）。</summary>
    private sealed class HeaderRow : Element
    {
        private readonly Label titleLabel;
        private readonly Button closeButton;
        private readonly float spacing;

        /// <summary>构造标题行。</summary>
        /// <param name="titleLabel">标题标签。</param>
        /// <param name="closeButton">关闭按钮。</param>
        /// <param name="spacing">标题与按钮之间的间隔。</param>
        public HeaderRow(Label titleLabel, Button closeButton, float spacing)
        {
            this.titleLabel = titleLabel;
            this.closeButton = closeButton;
            this.spacing = spacing;
            this.Add(this.titleLabel);
            this.Add(this.closeButton);
        }

        /// <inheritdoc />
        protected override Vector2 MeasureOverride(Vector2 available)
        {
            var buttonSize = this.closeButton.Measure(available);
            var titleAvailable = Math.Max(1f, available.X - buttonSize.X - this.spacing);
            var titleSize = this.titleLabel.Measure(new Vector2(titleAvailable, available.Y));

            return new Vector2(titleSize.X + this.spacing + buttonSize.X, Math.Max(titleSize.Y, buttonSize.Y));
        }

        /// <inheritdoc />
        protected override void ArrangeOverride(Rectangle final)
        {
            var buttonWidth = (int)this.closeButton.DesiredSize.X;
            var buttonHeight = (int)this.closeButton.DesiredSize.Y;
            var titleHeight = (int)this.titleLabel.DesiredSize.Y;

            // 两者都在这行的垂直中线上：标题与按钮高度由各自字体/内边距决定，顶对齐会看着歪
            this.closeButton.Arrange(new Rectangle(
                final.Right - buttonWidth,
                final.Y + (final.Height - buttonHeight) / 2,
                buttonWidth,
                buttonHeight));
            this.titleLabel.Arrange(new Rectangle(
                final.X,
                final.Y + (final.Height - titleHeight) / 2,
                Math.Max(0, final.Width - buttonWidth - (int)this.spacing),
                titleHeight));
        }
    }

    private readonly BlockerButton blocker = new();
    private readonly PanelFrame panel = new();
    private readonly Stack content = new(Stack.Direction.Vertical, Spacing);
    private readonly Label titleLabel = new(string.Empty);
    private readonly Button closeButton = new(string.Empty);
    private readonly Label metaLabel = new(string.Empty);
    private readonly Scrollable list = new(width: 0f, height: ListHeight, spacing: SectionSpacing);
    private readonly HeaderRow header;

    /// <summary>本帧的面板尺寸（测量时定下，布置时按它居中）。</summary>
    private Vector2 panelSize;

    /// <summary>构造窗口（内容留空，由 <see cref="Sync" /> 按当前语言与差异结果写入）。</summary>
    /// <param name="onCloseRequest">请求关闭时执行的动作（关闭按钮与 <c>Esc</c> 共用）。</param>
    public ModInfoWindow(Action onCloseRequest)
    {
        this.header = new HeaderRow(this.titleLabel, this.closeButton, Spacing);
        this.closeButton.OnClick = onCloseRequest;
        this.panel.SetContent(this.content);
        // 拦截层排在面板之前：宿主取「最上层命中」，后加入的面板因此仍能拿到面板上的点击
        this.Add(this.blocker);
        this.Add(this.panel);
    }

    /// <summary>按差异结果重建内容（打开窗口时、以及打开期间差异结果换了实例时调用；列表回到顶部）。</summary>
    /// <param name="diff">该存档的差异结果。</param>
    public void Sync(ModDiff diff)
    {
        this.titleLabel.Text = I18n.UI_ModInfo_WindowTitle();
        this.closeButton.Text = I18n.UI_ModInfo_CloseButton();

        this.content.Clear();
        this.content.Add(this.header);

        // 没有记录：没有任何可列的明细，窗口由「无法对比」的说明行收尾（说明里带上「载入并保存一次」这条出路）
        if (!diff.HasRecord)
        {
            this.content.Add(new Label(I18n.UI_ModInfo_NoRecord()));

            return;
        }

        // 元信息行紧跟标题：记录时间、记录时的游戏版本、模组数怎么变的（记录时 -> 当前）
        this.metaLabel.Text = I18n.UI_ModInfo_MetaLine(diff.RecordedAt, diff.GameVersion, diff.RecordedModCount, diff.CurrentModCount);
        this.content.Add(this.metaLabel);

        this.RebuildSections(diff);
        this.content.Add(this.list);
        this.list.ScrollTo(0f);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        // 铺满视口（拦截层与压暗都是全屏的）
        this.blocker.Measure(available);

        // 面板宽度定死（模组名长短不会让窗口忽宽忽窄），高度随内容生长并封顶于视口留边后的上限：
        // 没有记录时窗口自然收短成「一句说明」的高度，不会留一大片空面板
        var maxSize = this.GetMaxPanelSize(available);
        var desired = this.panel.Measure(new Vector2(maxSize.X, maxSize.Y));

        this.panelSize = new Vector2(maxSize.X, Math.Min(desired.Y, maxSize.Y));

        return new Vector2(available.X, available.Y);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        this.blocker.Arrange(final);

        this.panel.Arrange(new Rectangle(
            final.X + (final.Width - (int)this.panelSize.X) / 2,
            final.Y + (final.Height - (int)this.panelSize.Y) / 2,
            (int)this.panelSize.X,
            (int)this.panelSize.Y));
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        // 铺满视口的压暗层：画在本层最底，面板由子级随后画在其上
        batch.Draw(Game1.staminaRect, this.Bounds, Theme.OverlayDim);
    }

    /// <summary>按视口算面板的尺寸上限（宽度固定，高度封顶；两者都夹进视口留边）。</summary>
    /// <param name="available">可用空间（视口尺寸）。</param>
    /// <returns>面板尺寸上限。</returns>
    private Vector2 GetMaxPanelSize(Vector2 available)
    {
        return new Vector2(
            Math.Max(1f, Math.Min(PanelWidth, available.X - ViewportMargin * 2)),
            Math.Max(1f, Math.Min(PanelHeight, available.Y - ViewportMargin * 2)));
    }

    /// <summary>重建分节明细：三类各一节，空节不挂（玩家看到的只有真的发生过的那几类）。</summary>
    /// <param name="diff">差异结果。</param>
    private void RebuildSections(ModDiff diff)
    {
        this.list.Clear();

        // 「已更新」的条目要拼出「名称（旧 -> 新）」；另两类的条目只放显示名，归属已经由节标题说清了
        var updatedLines = diff.Updated
            .Select(change => I18n.UI_ModInfo_UpdatedMod(change.Name, change.OldVersion, change.NewVersion))
            .ToList();

        this.AddSection(I18n.UI_ModInfo_RemovedSection(diff.Removed.Count), diff.Removed);
        this.AddSection(I18n.UI_ModInfo_AddedSection(diff.Added.Count), diff.Added);
        this.AddSection(I18n.UI_ModInfo_UpdatedSection(updatedLines.Count), updatedLines);
    }

    /// <summary>挂一个分节（节标题 + 条目行），节标题用对话框字体与条目行拉开层次。</summary>
    /// <param name="header">节标题（已带计数）。</param>
    /// <param name="items">该节的条目行。</param>
    private void AddSection(string header, IReadOnlyList<string> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        var section = new Stack(Stack.Direction.Vertical, ItemSpacing);
        section.Add(new Label(header) { Font = Theme.DialogueFont });

        foreach (var item in items)
        {
            section.Add(new Label(item));
        }

        this.list.Add(section);
    }
}
