using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.ReadyCheckKick.UI;

/// <summary>
/// 未准备玩家列表的一行：左侧原版迷你头像，中间玩家名，右侧可选的「踢出」按钮。
/// 行的视觉自绘，交互只有那枚按钮——只读形态不传 <c>kickFarmer</c>，行内便没有任何可点的东西。
/// </summary>
internal sealed class UnreadyFarmerRow : Element
{
    /// <summary>
    /// 行内「踢出」按钮：比框架默认按钮紧凑一圈，姿势同框架自己的页签芯片（<c>TabControl.TabChip</c>：
    /// 覆写测量用紧凑内边距，盒体、悬停与点击视觉全部沿用框架默认）。默认内边距（48 × 24）会把按钮撑到
    /// 73px 高（高于 <see cref="MinRowHeight" />，行高随之变成 73）、并把名字列挤到不足 100px；
    /// 紧凑后按钮 49px 高，行高仍是 <see cref="MinRowHeight" />，面板的「可见五行」也随之成立。
    /// </summary>
    private sealed class KickButton : Button
    {
        /// <summary>文本左右内边距（按钮最小宽度 = 文本宽 + 2×该值）。</summary>
        private const float HorizontalPadding = 16f;

        /// <summary>文本上下内边距（按钮最小高度 = 文本高 + 2×该值）。</summary>
        private const float VerticalPadding = 12f;

        /// <summary>构造按钮。</summary>
        /// <param name="text">按钮文本。</param>
        public KickButton(string text)
            : base(text) { }

        /// <summary>以紧凑内边距测量（见本类注释）。</summary>
        /// <inheritdoc />
        protected override Vector2 MeasureOverride(Vector2 available)
        {
            var size = Theme.SmallFont.MeasureString(this.Text);

            return new Vector2(
                Math.Min(size.X + HorizontalPadding * 2f, available.X),
                Math.Min(size.Y + VerticalPadding * 2f, available.Y)
            );
        }
    }

    /// <summary>
    /// 常规行高（名字单行时）。名字折成多行时行会跟着变高，故这是**最小**行高。
    /// 头像缩放取「常规行高 / 16」（原版头像底图 16px 见方），故头像与常规行同高。
    /// 原版过夜状态列表以 <c>draw_scale</c> 4 绘制：行距 16 × 4 = 64px（<c>PlayerStatusList.Draw</c> 在 Icons 模式
    /// 拿 <c>largestSpriteHeight</c> 当行距）、头像 4 × 0.75 × 16 = 48px，即头像只占行距的 0.75；
    /// 本面板的行高比原版行距矮，头像取满行高才看得清。
    /// </summary>
    public const float MinRowHeight = 56f;

    /// <summary>头像与名字之间的间隙。</summary>
    private const float PortraitGap = 8f;

    /// <summary>名字与「踢出」按钮之间的间隙。</summary>
    private const float ButtonGap = 8f;

    /// <summary>头像缩放（原版头像底图 16px 宽，故头像像素宽 = 16 × 本值）。</summary>
    private const float PortraitScale = MinRowHeight / 16f;

    /// <summary>头像在行内占用的横向槽位：不论有无农民引用都留出，行与行的名字左缘才对得齐。</summary>
    private const float PortraitSlotWidth = 16f * PortraitScale + PortraitGap;

    /// <summary>
    /// 头像绘制深度：取菜单内惯例的小值（<c>GameMenu.draw</c> 画玩家头像用的 0.00011f），
    /// 以菜单内其它 UI 的深度为基准，避免叠层头像被压在下面。
    /// </summary>
    private const float PortraitLayerDepth = 0.00011f;

    private readonly Label nameLabel;
    private readonly Farmer? farmer;
    private readonly Button? kickButton;

    /// <summary>构造一行。</summary>
    /// <param name="unreadyFarmer">本行要呈现的数据。</param>
    /// <param name="kickFarmer">点击「踢出」时要执行的动作（接收本行玩家）；null = 只读形态，行内不建按钮。</param>
    public UnreadyFarmerRow(UnreadyFarmer unreadyFarmer, Action<UnreadyFarmer>? kickFarmer = null)
    {
        this.farmer = unreadyFarmer.Farmer;
        this.nameLabel = new Label(unreadyFarmer.DisplayName);
        this.Add(this.nameLabel);

        if (kickFarmer is not null)
        {
            // 文案留空占位：真实文案由面板每帧现取写入（构造期取一次会留下英文基文案，见 SyncKickButtonText）
            this.kickButton = new KickButton(string.Empty)
            {
                OnClick = () => kickFarmer(unreadyFarmer)
            };
            this.Add(this.kickButton);
        }
    }

    /// <summary>
    /// 把「踢出」按钮的文案同步到当前语言。只在真正变化时写入——<see cref="Button.Text" /> 的 setter 会标脏重排，
    /// 每帧无条件写会让布局每帧重跑。
    /// </summary>
    /// <param name="text">按钮文案（由面板每帧现取，理由同面板标题）。</param>
    public void SyncKickButtonText(string text)
    {
        if (this.kickButton is not null && !string.Equals(this.kickButton.Text, text, StringComparison.Ordinal))
        {
            this.kickButton.Text = text;
        }
    }

    /// <summary>「踢出」按钮在行内占用的横向空间（含其前的间隙）；只读形态为 0。</summary>
    private float ReservedButtonWidth => this.kickButton is null ? 0f : this.kickButton.DesiredSize.X + ButtonGap;

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        // 按钮先测：它的宽度要从名字的可用宽度里扣掉（就地测量，容器不会替它测）
        this.kickButton?.Measure(new Vector2(available.X, float.MaxValue));

        // 名字按「去掉头像槽位与按钮后的宽度」测量：过长时由 Label 自行折行，不撑破面板的固定宽度
        var nameWidth = Math.Max(1f, available.X - PortraitSlotWidth - this.ReservedButtonWidth);
        this.nameLabel.Measure(new Vector2(nameWidth, float.MaxValue));

        // 折成多行的名字要把行撑高：行高定死会让文本画到上一行去（垂直居中偏移变负），下一行的内容也会被压住
        return new Vector2(available.X, Math.Max(MinRowHeight, this.nameLabel.DesiredSize.Y));
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        var nameWidth = Math.Max(0f, final.Width - PortraitSlotWidth - this.ReservedButtonWidth);
        var nameHeight = this.nameLabel.DesiredSize.Y;

        // 名字在行内垂直居中（文本行高随语言/字体浮动，居中比贴顶稳；行高已被撑到不低于文本高，偏移恒非负）
        this.nameLabel.Arrange(new Rectangle(
            final.X + (int)PortraitSlotWidth,
            final.Y + (int)((final.Height - nameHeight) / 2f),
            (int)nameWidth,
            (int)nameHeight));

        if (this.kickButton is null)
        {
            return;
        }

        // 按钮贴行右缘、行内垂直居中
        var buttonSize = this.kickButton.DesiredSize;

        this.kickButton.Arrange(new Rectangle(
            final.Right - (int)buttonSize.X,
            final.Y + (int)((final.Height - buttonSize.Y) / 2f),
            (int)buttonSize.X,
            (int)buttonSize.Y));
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        if (this.farmer is null)
        {
            return;
        }

        // 原版签名里的 facingDirection 内部被硬写为 2（正脸），此处照传；alpha 用默认的全不透明
        this.farmer.FarmerRenderer.drawMiniPortrat(batch, new Vector2(this.Bounds.X, this.Bounds.Y), PortraitLayerDepth, PortraitScale, 2, this.farmer);
    }
}
