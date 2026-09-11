using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.BetterCabin.UI;

/// <summary>
/// 建筑外观预览区：左右两个箭头图标按钮 + 底部外观计数，中间按 4× 绘制建筑。
/// 预览必须是自定义元素：框架 <see cref="Stack" /> 把完整主轴尺寸交给每个子级（贪婪子级会吃掉宽度、把后续兄弟挤成 0），
/// <see cref="Grid" /> 格子等大（做不出"箭头 / 宽预览 / 箭头"），所以箭头与计数由本元素自管 Arrange。
/// 建筑用 <see cref="Building.drawInMenu" /> 绘制，它固定 4× 且不接受缩放参数（游戏源码 <c>StardewValley.Buildings/Building.cs:2171</c>
/// 的签名只收 (SpriteBatch, int, int)，:2190 内部硬编码 <c>scale: 4f</c>），因此预览高度在构造时按
/// <see cref="MeasureArtworkHeight" /> 定死，窗口高度随之自适应；视口装不下时由窗口居中裁切兜底。
/// </summary>
internal class CabinPreview : Element
{
    /// <summary>
    /// 箭头图标按钮：透明 <see cref="Button" /> 承接悬停 / 点击 / 焦点环，箭头图标画在它之上。
    /// 绘制顺序是"先子级后图标"——框架按钮的焦点 / 悬停环是实心金板（透明盒盖不住内芯），
    /// 只有让图标盖上去才留下"金边"观感（与仓库内 AMA 的 OptionTile 同一姿势）。
    /// </summary>
    private sealed class ArrowButton : Element
    {
        /// <summary>透明按钮：盒体不绘制（透明度 0），只保留悬停 / 点击 / 焦点环。</summary>
        private sealed class TransparentButton : Button
        {
            public TransparentButton() : base(string.Empty) { }

            /// <summary>盒体完全透明：箭头图标由外层绘制层呈现。</summary>
            protected override float BoxOpacity => 0f;
        }

        private readonly TransparentButton button = new();
        private readonly Rectangle iconSource;

        /// <summary>构造箭头图标按钮。</summary>
        /// <param name="tileIndex">箭头在 mouseCursors 标准图块表里的下标。</param>
        /// <param name="onClick">点击 / 手柄 A 激活的动作。</param>
        /// <param name="tooltip">悬停 / 获焦提示文案。</param>
        public ArrowButton(int tileIndex, Action onClick, string tooltip)
        {
            this.iconSource = Game1.getSourceRectForStandardTileSheet(Theme.MouseCursors, tileIndex);
            this.button.OnClick = onClick;
            this.button.TooltipText = tooltip;
            this.Add(this.button);
        }

        /// <summary>承接悬停 / 点击 / 焦点环的透明按钮（箭头图标由本元素铺在它之上）。</summary>
        public Button HitButton => this.button;

        /// <inheritdoc />
        protected override Vector2 MeasureOverride(Vector2 available)
        {
            this.button.Measure(new Vector2(ArrowSize, ArrowSize));

            return new Vector2(ArrowSize, ArrowSize);
        }

        /// <inheritdoc />
        protected override void ArrangeOverride(Rectangle final)
        {
            // 透明按钮铺满整块：悬停 / 点击 / 焦点环的命中面与图标等大
            this.button.Arrange(final);
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch batch)
        {
            if (!this.Visible)
            {
                return;
            }

            // 先让子级（透明按钮）画悬停 / 焦点环，再在其上铺箭头图标（图标盖出金边；本元素无自绘内容）
            base.Draw(batch);

            batch.Draw(Theme.MouseCursors, new Vector2(this.Bounds.X, this.Bounds.Y), this.iconSource, Color.White, 0f, Vector2.Zero,
                (float)this.Bounds.Width / this.iconSource.Width, SpriteEffects.None, 0f);
        }
    }

    /// <summary>建筑绘制倍率：菜单内的建筑一律 4×，与 <see cref="Building.drawInMenu" /> 一致。</summary>
    private const int PreviewScale = 4;

    /// <summary>箭头按钮边长（原版 16×16 箭头图标按 4× 铺满该方块）。</summary>
    private const int ArrowSize = 64;

    /// <summary>「上一种外观」的箭头图块（mouseCursors 标准图块，原版建筑皮肤菜单同款）。</summary>
    private const int PreviousArrowTile = 44;

    /// <summary>「下一种外观」的箭头图块。</summary>
    private const int NextArrowTile = 33;

    private readonly Building building;
    private readonly ArrowButton previousArrow;
    private readonly ArrowButton nextArrow;
    private readonly Label counter;

    /// <summary>建筑美术占用的高度（<see cref="MeasureArtworkHeight" />）。</summary>
    private readonly int artworkHeight;

    /// <summary>底部计数行占用的高度（measure 时按计数文案的实际行高刷新）。</summary>
    private float counterHeight;

    /// <summary>「上一种外观」箭头按钮（供根视图的动作开关统一置空 / 还原点击动作）。</summary>
    public Button PreviousButton => this.previousArrow.HitButton;

    /// <summary>「下一种外观」箭头按钮（同上）。</summary>
    public Button NextButton => this.nextArrow.HitButton;

    /// <summary>构造预览区。</summary>
    /// <param name="building">要预览的建筑（外观切换后同一实例的源矩形即为新外观）。</param>
    /// <param name="onPrevious">切到上一种外观。</param>
    /// <param name="onNext">切到下一种外观。</param>
    public CabinPreview(Building building, Action onPrevious, Action onNext)
    {
        this.building = building;
        this.artworkHeight = MeasureArtworkHeight(building);

        this.previousArrow = new ArrowButton(PreviousArrowTile, onPrevious, I18n.UI_ClientCabinMenu_PreviousSkin());
        this.nextArrow = new ArrowButton(NextArrowTile, onNext, I18n.UI_ClientCabinMenu_NextSkin());
        this.counter = new Label(string.Empty, center: true);

        this.Add(this.previousArrow);
        this.Add(this.nextArrow);
        this.Add(this.counter);
    }

    /// <summary>建筑美术占用的高度（4× 后的源矩形高）：窗口高度按它 + 其余留白算出。</summary>
    /// <param name="building">预览的建筑。</param>
    /// <returns>美术高度（像素）。</returns>
    public static int MeasureArtworkHeight(Building building)
    {
        return building.getSourceRect().Height * PreviewScale;
    }

    /// <summary>刷新底部外观计数。</summary>
    /// <param name="index">当前外观下标（从 0 起；展示时换成从 1 起的序号）。</param>
    /// <param name="count">外观总数。</param>
    public void SetCounter(int index, int count)
    {
        this.counter.Text = I18n.UI_ClientCabinMenu_SkinCount(index + 1, count);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        this.previousArrow.Measure(new Vector2(ArrowSize, ArrowSize));
        this.nextArrow.Measure(new Vector2(ArrowSize, ArrowSize));

        // 计数文案可以为空（构造后由根视图立刻写入），行高取实测行高与字体行高的较大值，避免空文本把行压成 0
        var counterSize = this.counter.Measure(new Vector2(available.X, available.Y));
        this.counterHeight = Math.Max(counterSize.Y, Theme.SmallFont.LineSpacing);

        // 预览区撑满可用宽度：箭头贴左右边缘、建筑居中绘制，等价于原版"宽预览 + 两侧箭头"
        return new Vector2(available.X, this.artworkHeight + this.counterHeight);
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 计数行贴底而不是贴"美术高度"处：布局被挤压（视口封顶后放不下）时它仍留在面板内
        var counterTop = final.Bottom - (int)this.counterHeight;
        var arrowY = (final.Y + counterTop) / 2 - ArrowSize / 2;

        this.previousArrow.Arrange(new Rectangle(final.X, arrowY, ArrowSize, ArrowSize));
        this.nextArrow.Arrange(new Rectangle(final.Right - ArrowSize, arrowY, ArrowSize, ArrowSize));
        this.counter.Arrange(new Rectangle(final.X, counterTop, final.Width, (int)this.counterHeight));
    }

    /// <inheritdoc />
    protected override void DrawSelf(SpriteBatch batch)
    {
        // 美术在占位区里居中：源矩形高与构造时一致时正好铺满，外观换了尺寸也仍然居中
        var sourceRect = this.building.getSourceRect();
        var x = this.Bounds.Center.X - sourceRect.Width * PreviewScale / 2;
        var y = this.Bounds.Y + (this.artworkHeight - sourceRect.Height * PreviewScale) / 2;

        this.building.drawInMenu(batch, x, y);
    }
}
