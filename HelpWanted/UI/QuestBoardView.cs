using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Quests;
using weizinai.StardewValleyMod.HelpWanted.Model;
using weizinai.StardewValleyMod.PiCore.UI.Layout;
using weizinai.StardewValleyMod.PiCore.UI.Widget;

namespace weizinai.StardewValleyMod.HelpWanted.UI;

/// <summary>
/// 板面根视图：画木框内的板面装饰（空板文案、星星、祝尼魔、奖券与 <c>x1</c> 计数）与两态内容
/// （板面态的便签 / 详情态的任务描述与 Accept 按钮）。
/// 所有内容都按<strong>菜单盒左上角</strong>的绝对坐标定位：宿主把根视图布置在内缩 24px 的内容矩形
/// （<c>MenuHost.ContentRect</c>）里，若拿 <see cref="Element.Bounds" /> 当板面原点，板面 / 装饰 / 便签会整体跟着内缩挪位。
/// 便签位置不由本视图计算：管理器摆好的矩形直接写回便签（<see cref="QuestNote.PlacedBounds" />）。
/// </summary>
internal class QuestBoardView : Element
{
    /// <summary>空板文案在菜单盒内的偏移，沿用旧实现。</summary>
    private const int NothingPostedOffsetX = 384;

    /// <summary>空板文案在菜单盒内的偏移，沿用旧实现。</summary>
    private const int NothingPostedOffsetY = 320;

    /// <summary>任务描述在菜单盒内的偏移，沿用旧实现。</summary>
    private const int DescriptionOffsetX = 320 + 32;

    /// <summary>任务描述在菜单盒内的偏移，沿用旧实现。</summary>
    private const int DescriptionOffsetY = 256;

    /// <summary>任务描述的折行可用宽度，与旧实现的 <c>Game1.parseText(..., 640)</c> 对齐。</summary>
    private const float DescriptionMaxWidth = 640f;

    /// <summary>星星装饰相对前一颗的水平间距（沿用旧实现）。</summary>
    private const float StarSpacing = 12f;

    /// <summary>奖券 <c>x1</c> 计数相对菜单盒左上角的偏移，沿用旧实现。</summary>
    private const int TicketCountOffsetX = 936;

    /// <summary>奖券 <c>x1</c> 计数相对菜单盒左上角的偏移，沿用旧实现。</summary>
    private const int TicketCountOffsetY = 596;

    /// <summary>布告栏贴图的绘制倍率（原版贴图按 4 倍缩放画，装饰的偏移同样按此倍率放大）。</summary>
    private const float BillboardScale = 4f;

    /// <summary>三处装饰的绘制层深，沿用旧实现（压在便签与贴图之上）。</summary>
    private const float DecorationDepth = 0.6f;

    /// <summary>星星装饰首颗相对菜单盒左上角的偏移与源区域，沿用旧实现。</summary>
    private static readonly Vector2 StarOffset = new(18f, 36f);

    /// <summary>星星的贴图源区域，沿用旧实现。</summary>
    private static readonly Rectangle StarSourceRect = new(140, 397, 10, 11);

    /// <summary>祝尼魔相对菜单盒左上角的偏移，沿用旧实现。</summary>
    private static readonly Vector2 JunimoOffset = new(290f, 59f);

    /// <summary>祝尼魔的贴图源区域，沿用旧实现。</summary>
    private static readonly Rectangle JunimoSourceRect = new(0, 427, 39, 54);

    /// <summary>奖券图标相对菜单盒左上角的偏移，沿用旧实现。</summary>
    private static readonly Vector2 TicketIconOffset = new(215f, 144f);

    /// <summary>奖券图标在 <c>TileSheets/Objects_2</c> 上的源区域，沿用旧实现。</summary>
    private static readonly Rectangle TicketIconSourceRect = new(80, 128, 16, 16);

    private readonly BoardType boardType;
    private readonly Texture2D billboardTexture;
    private readonly Vector2 boardSize;
    private readonly List<QuestNote> notes = new();

    private Point menuOrigin;
    private bool showingDetail;
    private Label? description;
    private Button? acceptButton;
    private Rectangle acceptBounds;

    /// <summary>菜单盒左上角（屏幕绝对坐标）：板面装饰与详情态内容都相对它定位。</summary>
    private Vector2 Position => new(this.menuOrigin.X, this.menuOrigin.Y);

    /// <summary>构造板面根视图。</summary>
    /// <param name="boardType">板子的类型（决定星星 / 祝尼魔 / 奖券三处装饰是否绘制）。</param>
    /// <param name="billboardTexture">布告栏贴图（星星、祝尼魔与奖券与板面共用同一张图）。</param>
    /// <param name="boardSize">板面尺寸（由宿主给出，与菜单盒同尺寸）。</param>
    public QuestBoardView(BoardType boardType, Texture2D billboardTexture, Vector2 boardSize)
    {
        ArgumentNullException.ThrowIfNull(billboardTexture);

        this.boardType = boardType;
        this.billboardTexture = billboardTexture;
        this.boardSize = boardSize;
    }

    /// <summary>
    /// 板面态：按管理器算好的位置展示当天还没被接下的便签。
    /// </summary>
    /// <param name="menuOrigin">菜单盒左上角（屏幕绝对坐标）。</param>
    /// <param name="pendingNotes">当天待接的便签（位置由管理器算好）。</param>
    /// <param name="onActivate">便签被点击 / 手柄 A 激活时的动作。</param>
    public void ShowNotes(Point menuOrigin, IReadOnlyList<QuestNote> pendingNotes, Action<QuestNote> onActivate)
    {
        ArgumentNullException.ThrowIfNull(pendingNotes);
        ArgumentNullException.ThrowIfNull(onActivate);

        this.menuOrigin = menuOrigin;
        this.Clear();
        this.notes.Clear();
        this.description = null;
        this.acceptButton = null;
        this.showingDetail = false;

        foreach (var note in pendingNotes)
        {
            // 便签实例由管理器持有、当天跨开板复用（同一天重复开板位置不变），上一次那个视图对象可能已经连着便签一起被丢弃，
            // 便签身上却还挂着旧父级——框架的 Add 拒绝已挂父级的元素，故先摘掉旧父级再挂进本视图
            note.Parent?.Remove(note);
            note.OnActivate = () => onActivate(note);
            this.notes.Add(note);
            this.Add(note);
        }
    }

    /// <summary>
    /// 详情态：原地把板面换成任务描述与 Accept 按钮（便签一并撤下，与旧实现一致，不叠层、不新开一页）。
    /// </summary>
    /// <param name="menuOrigin">菜单盒左上角（屏幕绝对坐标）。</param>
    /// <param name="quest">要展示的任务。</param>
    /// <param name="acceptBounds">Accept 按钮矩形（沿用旧算式，由宿主算好）。</param>
    /// <param name="onAccept">Accept 被点击 / 手柄 A 激活时的动作。</param>
    public void ShowQuest(Point menuOrigin, Quest quest, Rectangle acceptBounds, Action onAccept)
    {
        ArgumentNullException.ThrowIfNull(quest);
        ArgumentNullException.ThrowIfNull(onAccept);

        this.menuOrigin = menuOrigin;
        this.Clear();
        this.notes.Clear();
        this.showingDetail = true;
        this.acceptBounds = acceptBounds;

        // 韩文详情仍用 smallFont（沿用旧特例，避免韩文在对话字体下挤成一团）；描述里的性别分支块走游戏自己的规则
        //（旧实现经 Game1.parseText 同样如此），故此处不做 '^' 替换：'^' 是性别分支块内的分隔符，硬换行是 '\n'
        this.description = new Label(Dialogue.applyGenderSwitchBlocks(Game1.player.Gender, quest.questDescription))
        {
            Font = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont,
            MaxWidth = DescriptionMaxWidth,
        };
        this.acceptButton = new Button(Game1.content.LoadString("Strings\\UI:AcceptQuest"))
        {
            OnClick = onAccept,
        };

        this.Add(this.description);
        this.Add(this.acceptButton);
    }

    /// <inheritdoc />
    public override void Draw(SpriteBatch batch)
    {
        if (!this.Visible)
        {
            return;
        }

        // 先画子级（便签 / 详情态内容），再画板面装饰：与旧实现的绘制顺序一致
        foreach (var child in this.Children)
        {
            if (child.Visible)
            {
                child.Draw(batch);
            }
        }

        this.DrawDecorations(batch);
    }

    /// <inheritdoc />
    protected override Vector2 MeasureOverride(Vector2 available)
    {
        // 板面尺寸固定（由宿主给出）；子级（详情态的文本要折行、按钮要量文本）必须先量过，Arrange 时才有真实期望尺寸
        foreach (var child in this.Children)
        {
            child.Measure(this.boardSize);
        }

        return this.boardSize;
    }

    /// <inheritdoc />
    protected override void ArrangeOverride(Rectangle final)
    {
        // 便签按管理器算好的绝对矩形布置（不参与本视图的相对布局）；详情态的内容按旧坐标摆在板面上
        foreach (var note in this.notes)
        {
            note.Arrange(note.PlacedBounds);
        }

        if (this.description is not null)
        {
            var size = this.description.DesiredSize;
            var bounds = new Rectangle(
                this.menuOrigin.X + DescriptionOffsetX,
                this.menuOrigin.Y + DescriptionOffsetY,
                (int)size.X,
                (int)size.Y
            );
            this.description.Arrange(bounds);
        }

        if (this.acceptButton is not null)
        {
            this.acceptButton.Arrange(this.acceptBounds);
        }
    }

    /// <summary>画板面装饰：空板文案、奖券与 <c>x1</c> 计数、星星、祝尼魔，位置与层深全部沿用旧实现。</summary>
    /// <param name="batch">精灵批。</param>
    private void DrawDecorations(SpriteBatch batch)
    {
        // 空板文案：板上没有便签时画原版“今天没有张贴任何东西”（详情态必有便签，不会走到这里）
        if (!this.showingDetail && this.notes.Count == 0)
        {
            batch.DrawString(
                Game1.dialogueFont,
                Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"),
                new Vector2(this.menuOrigin.X + NothingPostedOffsetX, this.menuOrigin.Y + NothingPostedOffsetY),
                Game1.textColor
            );
        }

        // 奖券只在详情态画（旧实现就在详情里画），星星与祝尼魔是原版板的常驻装饰
        if (this.showingDetail && this.boardType == BoardType.Vanilla)
        {
            this.DrawTicket(batch);
        }

        if (this.boardType != BoardType.Vanilla)
        {
            return;
        }

        this.DrawStars(batch);
        this.DrawJunimo(batch);
    }

    /// <summary>画布告栏任务进度星星：完成数每 3 个一颗。</summary>
    /// <param name="batch">精灵批。</param>
    private void DrawStars(SpriteBatch batch)
    {
        for (var i = 0; i < Game1.stats.Get("BillboardQuestsDone") % 3; i++)
        {
            batch.Draw(
                this.billboardTexture,
                this.Position + (StarOffset + new Vector2(StarSpacing * i, 0f)) * BillboardScale,
                StarSourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                BillboardScale,
                SpriteEffects.None,
                DecorationDepth
            );
        }
    }

    /// <summary>画完成社区中心后出现的祝尼魔。</summary>
    /// <param name="batch">精灵批。</param>
    private void DrawJunimo(SpriteBatch batch)
    {
        if (!Game1.player.hasCompletedCommunityCenter())
        {
            return;
        }

        batch.Draw(
            this.billboardTexture,
            this.Position + JunimoOffset * BillboardScale,
            JunimoSourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            BillboardScale,
            SpriteEffects.None,
            DecorationDepth
        );
    }

    /// <summary>画接满 3 个布告栏任务时出现的奖券图标与 <c>x1</c> 计数。</summary>
    /// <param name="batch">精灵批。</param>
    private void DrawTicket(SpriteBatch batch)
    {
        if (Game1.stats.Get("BillboardQuestsDone") % 3 != 2)
        {
            return;
        }

        Utility.drawWithShadow(
            batch,
            Game1.content.Load<Texture2D>("TileSheets\\Objects_2"),
            this.Position + TicketIconOffset * BillboardScale,
            TicketIconSourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            BillboardScale
        );
        SpriteText.drawString(batch, "x1", (int)this.Position.X + TicketCountOffsetX, (int)this.Position.Y + TicketCountOffsetY);
    }
}
