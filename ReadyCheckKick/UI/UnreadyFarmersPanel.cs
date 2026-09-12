using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ReadyCheckKick.UI;

/// <summary>
/// 未准备玩家面板：一整块扁平九宫格盒作面板底，盒内自上而下是居中的标题、可滚动列表，交互形态的末尾还有
/// 一枚「全部踢出」。每行是「头像 + 玩家名」，交互形态的行内另有一枚「踢出」。盒体由本元素自绘
/// （不用 <see cref="PanelFrame" /> 包，也不用 <see cref="Scrollable" /> 自带的盒），故面板是**一整块**盒而不是盒里套盒。
/// 宽度固定为 <see cref="Width" />：名字长度不定，宽度自适应会让面板每次刷新忽宽忽窄。
/// </summary>
/// <remarks>
/// **两种形态共用同一套视图与行组件**，由构造时是否给 <see cref="UnreadyFarmerActions" /> 决定：不给即只读形态
/// （过夜存盘面：只有头像与名字，行内与面板底都没有按钮），给了即交互形态（准备检查面：行内可点「踢出」、
/// 面板底可点「全部踢出」）。于是「过夜那一面只读」是**一处开关**而不是两份实现。
/// 视图与数据源解耦：本类只接收「玩家（id + 显示名 + 可选的 <c>Farmer</c> 引用）」的列表，
/// 不碰原版状态列表、不碰准备检查内部状态，也不决定何时显示（何时显示由调用方按活动菜单身份决定）。
/// 没有任何未准备玩家时本视图不做空态文案——整块面板由调用方隐藏。
/// </remarks>
internal sealed class UnreadyFarmersPanel : Element
{
    /// <summary>面板固定宽度（UI 像素）。调用方按它算右上角锚位。</summary>
    public const float Width = 360f;

    /// <summary>盒体边框到内容的留白。</summary>
    private const float Padding = 24f;

    /// <summary>标题、列表与「全部踢出」之间的间隔。</summary>
    private const float Spacing = 8f;

    /// <summary>列表同时可见的行数（再多就用鼠标滚轮翻）。</summary>
    private const int VisibleRows = 5;

    /// <summary>相邻两行的间隔（与 <see cref="Scrollable" /> 的默认子级间隔一致）。</summary>
    private const float RowSpacing = 8f;

    /// <summary>
    /// <see cref="Scrollable" /> 内容区相对其视口的内缩。框架里是 <see cref="Scrollable" /> 的私有常量，
    /// 此处按同一数值反算视口高，使「可见行数」这个常量成立。
    /// </summary>
    private const float ScrollContentInset = 24f;

    /// <summary>列表视口高：正好露出 <see cref="VisibleRows" /> 行（行高与行距之和，加内容区上下各一份内缩）。</summary>
    private const float ListViewportHeight = VisibleRows * UnreadyFarmerRow.MinRowHeight + (VisibleRows - 1) * RowSpacing + 2f * ScrollContentInset;

    private readonly UnreadyFarmerActions? actions;
    private readonly Label titleLabel;
    private readonly Scrollable list;
    private readonly Stack content;
    private readonly Button? kickAllButton;

    /// <summary>当前已建行的玩家（与树里的行一一对应；留一份是为了判定传入列表有没有真的变化）。</summary>
    private readonly List<UnreadyFarmer> shownFarmers = new();

    /// <summary>本帧的「踢出」文案（每帧现取，新建的行也用它，免得空文案先闪一帧）。</summary>
    private string kickFarmerText = string.Empty;

    /// <summary>
    /// 面板上是否有可点的东西（行内「踢出」与面板底「全部踢出」）。构造时给不给动作是**唯一**的形态开关，
    /// 叠层的悬停路由据此决定要不要发起命中——只读形态没有按钮，走一遍命中只会白跑。
    /// </summary>
    public bool IsInteractive => this.actions is not null;

    /// <summary>构造面板。文案留空，由 <see cref="Sync" /> 按当前语言逐帧写入。</summary>
    /// <param name="actions">交互形态要执行的踢出动作；null = 只读形态（行内与面板底都不建按钮）。</param>
    public UnreadyFarmersPanel(UnreadyFarmerActions? actions = null)
    {
        this.actions = actions;
        this.titleLabel = new Label(string.Empty, center: true);
        this.list = new Scrollable(width: 0f, height: ListViewportHeight, spacing: RowSpacing)
        {
            // 盒体由本元素自绘：再让列表画一次九宫格，面板里就成了盒里套盒
            DrawPanel = false
        };
        this.content = new Stack(Stack.Direction.Vertical, spacing: Spacing);
        this.content.Add(this.titleLabel);
        this.content.Add(this.list);

        // 「全部踢出」只在交互形态存在：只读形态的面板底不留任何可点的东西
        if (actions is not null)
        {
            this.kickAllButton = new Button(string.Empty)
            {
                OnClick = actions.Value.KickAllFarmers
            };
            this.content.Add(this.kickAllButton);
        }

        this.Add(this.content);
    }

    /// <summary>
    /// 同步面板内容：文案按当前语言现取，玩家列表仅在真正变化时重建行。每帧无条件重建会让布局每帧重跑，
    /// 面板尺寸与行位置随之抖动。
    /// </summary>
    /// <param name="farmers">当前未准备玩家（空列表时本视图保持原内容，是否隐藏由调用方决定）。</param>
    public void Sync(IReadOnlyList<UnreadyFarmer> farmers)
    {
        this.SyncTitle();
        this.SyncKickTexts();

        if (this.IsSameAs(farmers))
        {
            return;
        }

        this.shownFarmers.Clear();
        this.shownFarmers.AddRange(farmers);
        this.list.Clear();

        foreach (var farmer in this.shownFarmers)
        {
            var row = new UnreadyFarmerRow(farmer, this.actions?.KickFarmer);

            // 新建的行立刻拿本帧现取的文案：留到下一帧再补会先闪一帧空按钮
            row.SyncKickButtonText(this.kickFarmerText);
            this.list.Add(row);
        }
    }

    /// <summary>
    /// 把列表拉回顶部。滚动偏移是 retained 状态、会跨显示残留（框架只在布置时把它夹回合法范围），
    /// 每次重新显示时调一次，面板才不会从上一次留下的半截列表开始。
    /// </summary>
    public void ResetScroll()
    {
        this.list.ScrollTo(0f);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        var width = Math.Min(Width, available.X);
        this.content.Measure(new Vector2(Math.Max(1f, width - Padding * 2f), Math.Max(0f, available.Y - Padding * 2f)));

        // 宽度定死在上限内（不随内容变化）；高度取内容高：标题 + 固定高的列表视口（+ 交互形态的「全部踢出」）
        return new Vector2(Math.Min(width, this.content.DesiredSize.X + Padding * 2f), this.content.DesiredSize.Y + Padding * 2f);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        var inset = (int)Padding;

        this.content.Arrange(new Rectangle(
            final.X + inset,
            final.Y + inset,
            Math.Max(0, final.Width - inset * 2),
            Math.Max(0, final.Height - inset * 2)));
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        // 一整块扁平九宫格盒：盒体先画，内容（标题、列表与按钮）随后画在盒体之上
        Theme.DrawPanel(batch, this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);
    }

    /// <summary>
    /// 标题与两枚按钮文案按当前语言每帧现取而不缓存：模组的 <c>Entry</c> 早于游戏应用已保存的语言（SMAPI 在模组加载之后
    /// 才把翻译切到该语言），构造期取一次会把英文基文案永久留在界面上。仅在真正变化时写入——这几个 setter 都会标脏重排。
    /// </summary>
    private void SyncTitle()
    {
        var title = I18n.UI_UnreadyFarmersPanel_Title();

        if (!string.Equals(this.titleLabel.Text, title, StringComparison.Ordinal))
        {
            this.titleLabel.Text = title;
        }
    }

    /// <summary>
    /// 两枚踢出按钮的文案与标题同理每帧现取（理由见 <see cref="SyncTitle" />）；已建的行也一并跟上，
    /// 换语言后不必等列表变化。
    /// </summary>
    private void SyncKickTexts()
    {
        if (this.kickAllButton is not null)
        {
            var kickAllText = I18n.UI_KickAllButton();

            if (!string.Equals(this.kickAllButton.Text, kickAllText, StringComparison.Ordinal))
            {
                this.kickAllButton.Text = kickAllText;
            }
        }

        if (this.actions is null)
        {
            return;
        }

        this.kickFarmerText = I18n.UI_KickButton();

        foreach (var child in this.list.Children)
        {
            if (child is UnreadyFarmerRow row)
            {
                row.SyncKickButtonText(this.kickFarmerText);
            }
        }
    }

    /// <summary>
    /// 传入列表是否与面板当前内容一致：逐项比 id 与显示名。头像用的农民引用不参与判定——它只影响怎么画，
    /// 同一 id 的引用在会话内稳定，不构成内容变化。
    /// </summary>
    /// <param name="farmers">待比较的列表。</param>
    /// <returns>顺序、人员与名字都一致时为 true。</returns>
    private bool IsSameAs(IReadOnlyList<UnreadyFarmer> farmers)
    {
        if (this.shownFarmers.Count != farmers.Count)
        {
            return false;
        }

        for (var i = 0; i < this.shownFarmers.Count; i++)
        {
            var current = this.shownFarmers[i];
            var incoming = farmers[i];

            if (current.Id != incoming.Id || !string.Equals(current.DisplayName, incoming.DisplayName, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
