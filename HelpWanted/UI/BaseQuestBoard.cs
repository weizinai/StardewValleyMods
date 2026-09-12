using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.PiCore.UI;
using weizinai.StardewValleyMod.PiCore.UI.Host;
using weizinai.StardewValleyMod.PiCore.UI.Layout;

namespace weizinai.StardewValleyMod.HelpWanted.UI;

/// <summary>
/// 两块任务板（原版 / RSV）的共用宿主：<see cref="MenuHost" /> 子类，两态（板面 ↔ 详情）共用同一个菜单。
/// 覆写 <see cref="draw" /> 且<strong>不调用 base</strong>：布告栏贴图要铺满整个菜单盒，框架 chrome（九宫格外框）
/// 会露在贴图四周、框架提示框层也会跟着 chrome 一起丢，故两者一并跳过——提示框改回自绘 <c>drawHoverText</c>，
/// 悬停目标由本类自己命中（与 BetterCabin 移动态同一做法）。
/// 交互（悬停路由、左键命中、手柄焦点图与 A/B）全部交给框架，本类只保留四处覆写：
/// <see cref="draw" />、<see cref="performHoverAction" />（自绘提示的目标判定）、<see cref="receiveKeyPress" />
/// （详情态 Esc 退回板面）与 <see cref="receiveLeftClick" />（详情态点右上角 X 退回板面，沿用旧行为）。
/// 板面与便签的内容由 <see cref="QuestBoardView" /> 与 <see cref="QuestNote" /> 表达。
/// </summary>
public abstract class BaseQuestBoard : MenuHost
{
    /// <summary>菜单盒宽度（布告栏贴图 338 × 4 倍缩放）。</summary>
    private const int BoardWidth = 338 * 4;

    /// <summary>菜单盒高度（198 × 4 倍缩放）。</summary>
    private const int BoardHeight = 198 * 4;

    /// <summary>Accept 按钮在菜单盒内的水平位置（沿用旧算式：菜单盒中线再左移 128）。</summary>
    private const int AcceptButtonOffsetX = BoardWidth / 2 - 128;

    /// <summary>Accept 按钮距菜单盒底边的距离（沿用旧算式）。</summary>
    private const int AcceptButtonOffsetFromBottom = 128;

    /// <summary>Accept 按钮的文本内边距（沿用旧算式：文本尺寸 + 24）。</summary>
    private const int AcceptButtonPadding = 24;

    /// <summary>右上角 X 距菜单盒右边界的距离（沿用旧实现；框架默认值不同，故在构造里覆盖）。</summary>
    private const int CloseButtonOffsetFromRight = 20;

    /// <summary>板面（便签可摆放的区域）在菜单盒内的矩形，沿用旧实现。</summary>
    private static readonly Rectangle BoardRectInMenu = new(78 * 4, 52 * 4, 184 * 4, 102 * 4);

    /// <summary>右上角 X 的贴图源区域，沿用旧实现。</summary>
    private static readonly Rectangle CloseButtonSourceRect = new(337, 494, 12, 12);

    private readonly QuestBoardView view;
    private readonly IQuestManager questManager;
    private readonly Texture2D billboardTexture;
    private readonly Rectangle billboardTextureSourceRect;
    private readonly Rectangle acceptQuestButtonBounds;

    /// <summary>详情态正在展示的便签；null = 板面态。</summary>
    private QuestNote? showingNote;

    private string hoverTitle = "";
    private string hoverText = "";

    /// <summary>菜单盒左上角（屏幕绝对坐标）：板面、装饰与两态内容都相对它定位。</summary>
    private Point MenuOrigin => new(this.xPositionOnScreen, this.yPositionOnScreen);

    /// <summary>板面（便签可摆放的区域）在屏幕坐标系里的矩形。</summary>
    private Rectangle BoardScreenRect => new(
        this.xPositionOnScreen + BoardRectInMenu.X,
        this.yPositionOnScreen + BoardRectInMenu.Y,
        BoardRectInMenu.Width,
        BoardRectInMenu.Height
    );

    /// <summary>构造任务板菜单。</summary>
    /// <param name="boardType">板子的类型（原版 / RSV），决定便签的存放与三处装饰。</param>
    /// <param name="billboardTexture">布告栏贴图。</param>
    /// <param name="billboardTextureSourceRect">布告栏贴图要用的源区域。</param>
    protected BaseQuestBoard(BoardType boardType, Texture2D billboardTexture, Rectangle billboardTextureSourceRect)
        : base(new QuestBoardView(boardType, billboardTexture, new Vector2(BoardWidth, BoardHeight)), BoardWidth, BoardHeight)
    {
        this.view = (QuestBoardView)this.Root;
        this.billboardTexture = billboardTexture;
        this.billboardTextureSourceRect = billboardTextureSourceRect;

        // 关闭按钮：位置与贴图沿用旧实现（框架默认的 X 在 (宽-36, -8)，本板要 (宽-20, 0)）
        this.upperRightCloseButton = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width - CloseButtonOffsetFromRight, this.yPositionOnScreen, 48, 48),
            Game1.mouseCursors,
            CloseButtonSourceRect,
            4f
        );

        // Accept 按钮矩形：沿用旧算式，尺寸按对话字体量取的文案尺寸
        var acceptQuestTextSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));
        this.acceptQuestButtonBounds = new Rectangle(
            this.xPositionOnScreen + AcceptButtonOffsetX,
            this.yPositionOnScreen + this.height - AcceptButtonOffsetFromBottom,
            (int)acceptQuestTextSize.X + AcceptButtonPadding,
            (int)acceptQuestTextSize.Y + AcceptButtonPadding
        );

        // 待接便签逻辑：当天还没上板的待接任务交给管理器摆放，已经在板上的便签保持原有位置，
        // 因此当天重复开板不会重新随机摆放
        this.questManager = boardType == BoardType.Vanilla ? VanillaQuestManager.Instance : RSVQuestManager.Instance;
        this.questManager.PlacePendingNotes(this.BoardScreenRect);

        this.ShowBoardState();

        // 根视图是在基类构造里挂上的，此时才刚填好内容：先冲刷一次布局，之后任何读 Bounds 的代码（悬停命中、
        // 焦点图）拿到的都是真实屏幕坐标
        LayoutRunner.UpdateIfDirty(this.view, this.ContentRect);
    }

    /// <summary>
    /// 自绘整块菜单（不调用 base）：压暗背景（仍尊重“清空背景”选项）→ 布告栏贴图（4 倍缩放铺满菜单盒）→ 根视图
    /// （便签 / 详情态内容 + 板面装饰）→ 右上角 X → 鼠标光标 → 自绘悬停提示，顺序与旧手写菜单逐位一致。
    /// </summary>
    /// <param name="batch">精灵批。</param>
    public override void draw(SpriteBatch batch)
    {
        // 点击处理器可能在本次绘制前改动了树（点便签换详情态）：先冲刷挂起的脏布局再画，首帧就落在新排版上
        LayoutRunner.UpdateIfDirty(this.view, this.ContentRect);

        if (!Game1.options.showClearBackgrounds)
        {
            batch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Theme.OverlayDim);
        }

        batch.Draw(
            this.billboardTexture,
            new Vector2(this.xPositionOnScreen, this.yPositionOnScreen),
            this.billboardTextureSourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f
        );

        this.view.Draw(batch);

        // 右上角 X：base.draw 只画它，本方法不走 base，故自己画
        this.upperRightCloseButton?.draw(batch);

        Game1.mouseCursorTransparency = 1f;
        Theme.DrawMouseCursor(batch);

        if (this.hoverText.Length > 0)
        {
            drawHoverText(batch, this.hoverText, Game1.smallFont, 0, 0, -1, this.hoverTitle);
        }
    }

    /// <summary>
    /// 悬停：按钮悬停路由（悬停态与音效）交给框架宿主；便签的“标题 + 当前目标”提示仍沿用自绘
    /// （框架提示框层随 chrome 一起被 <see cref="draw" /> 跳过，故不用元素的提示文本），悬停目标由本方法自己命中。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);

        this.hoverTitle = "";
        this.hoverText = "";

        // 详情态下便签已撤下，不做便签悬停（旧实现同样只在板面态命中便签）
        if (this.showingNote is not null)
        {
            return;
        }

        foreach (var note in this.questManager.PendingNotes.Where(note => note.Bounds.Contains(x, y)))
        {
            this.hoverTitle = note.QuestModel.Quest.questTitle;
            this.hoverText = note.QuestModel.Quest.currentObjective;

            break;
        }
    }

    /// <summary>
    /// 键盘：详情态 Esc 退回板面（菜单保持打开），其余按键交给框架的键盘契约（只认 Esc 关闭；方向键不再移动焦点）。
    /// </summary>
    /// <param name="key">按下的键。</param>
    public override void receiveKeyPress(Keys key)
    {
        if (key == Keys.Escape && this.showingNote is not null)
        {
            this.ShowBoardState();

            return;
        }

        base.receiveKeyPress(key);
    }

    /// <summary>
    /// 鼠标左键：只有「详情态点右上角 X」需要拦下——旧实现在详情态点 X 是退回板面（菜单不关、只播一次关闭音），
    /// 而框架把右上角 X 的点击收在基类里（基类会直接关闭菜单），故这里先复现旧行为；
    /// 其余（板面态点 X 关闭、点便签、点 Accept）一律交给框架的左键路由。
    /// </summary>
    /// <param name="x">鼠标 X（UI 坐标）。</param>
    /// <param name="y">鼠标 Y（UI 坐标）。</param>
    /// <param name="playSound">是否播放音效。</param>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.showingNote is null || this.upperRightCloseButton is not { } closeButton || !closeButton.containsPoint(x, y))
        {
            base.receiveLeftClick(x, y, playSound);

            return;
        }

        if (playSound)
        {
            Game1.playSound(this.closeSound);
        }

        this.ShowBoardState();
    }

    /// <summary>回到板面态：按管理器算好的位置展示当天还没被接下的便签（开板、Esc 退回、接完任务都走这里）。</summary>
    private void ShowBoardState()
    {
        this.showingNote = null;
        this.view.ShowNotes(this.MenuOrigin, this.questManager.PendingNotes, this.OpenQuest);
    }

    /// <summary>进入详情态：便签原地换成任务描述 + Accept（与旧实现一致，不关菜单、不叠层）。</summary>
    /// <param name="note">被点开 / 手柄 A 激活的便签。</param>
    private void OpenQuest(QuestNote note)
    {
        this.showingNote = note;
        this.hoverTitle = "";
        this.hoverText = "";
        this.view.ShowQuest(this.MenuOrigin, note.QuestModel.Quest, this.acceptQuestButtonBounds, this.AcceptShowingQuest);
    }

    /// <summary>
    /// 接下详情态展示的任务：写进任务日志与 <c>dayQuestAccepted</c>，摘掉那张便签并回到板面态（便签随之消失）。
    /// 确认音由宿主对按钮点击 / 手柄 A 的统一路由播放（同一个 <c>newArtifact</c>），此处不再重复播放。
    /// </summary>
    private void AcceptShowingQuest()
    {
        if (this.showingNote is not { } note)
        {
            return;
        }

        var quest = note.QuestModel.Quest;
        quest.dayQuestAccepted.Value = Game1.Date.TotalDays;
        Game1.player.questLog.Add(quest);
        this.questManager.RemovePendingNote(note);

        this.ShowBoardState();
    }
}
