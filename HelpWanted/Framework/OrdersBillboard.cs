using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace HelpWanted.Framework;

public sealed class OrdersBillboard : Billboard
{
    public static readonly List<ClickableTextureComponent> QuestNotes = new();
    public static readonly Dictionary<int, IQuestData> QuestDataDictionary = new();
    private static Rectangle boardRect = new(78 * 4, 58 * 4, 184 * 4, 96 * 4);
    private const int OptionIndex = -4200;

    /// <summary>面板纹理</summary>
    private readonly Texture2D billboardTexture;

    /// <summary>正在展示的任务的ID</summary>
    public static int ShowingQuest;

    /// <summary>正在展示的任务的任务面板</summary>
    public static Billboard? QuestBillboard;

    // 悬浮标题和悬浮文本
    private string hoverTitle = "";
    private string hoverText = "";

    public OrdersBillboard() : base(true)
    {
        // 设置面板纹理
        billboardTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard");
        QuestBillboard = null;
        if (ModEntry.QuestList.Count > 0)
        {
            // 清空任务选项列表和任务数据字典
            QuestNotes.Clear();
            QuestDataDictionary.Clear();
            // 遍历所有的任务数据,创建任务选项
            var questList = ModEntry.QuestList;
            for (var i = 0; i < questList.Count; i++)
            {
                var size = new Point((int)(questList[i].PadTextureSource.Width * ModEntry.Config.NoteScale),
                    (int)(questList[i].PadTextureSource.Height * ModEntry.Config.NoteScale));
                var bounds = GetFreeBounds(size.X, size.Y);
                if (bounds is null) break;
                QuestNotes.Add(new ClickableTextureComponent(bounds.Value,
                    questList[i].PadTexture, questList[i].PadTextureSource, ModEntry.Config.NoteScale)
                {
                    // 设置该选项的ID
                    myID = OptionIndex - i,
                    // 如果该选项是最左侧的选项,则左邻居ID为-1,否则为当前选项ID+1
                    leftNeighborID = i > 0 ? OptionIndex - i + 1 : -1,
                    // 如果该选项是最右侧的选项,则右邻居ID为-1,否则为当前选项ID-1
                    rightNeighborID = i < questList.Count - 1 ? OptionIndex - i - 1 : -1
                });
                QuestDataDictionary[OptionIndex - i] = questList[i];
            }

            ModEntry.QuestList.Clear();
        }

        exitFunction = delegate
        {
            if (QuestBillboard is not null)
                Game1.activeClickableMenu = new OrdersBillboard();
        };
        // 获得所有的可点击组件
        populateClickableComponentList();
    }

    /// <summary>处理鼠标悬停事件</summary>
    public override void performHoverAction(int x, int y)
    {
        // 如果目前正在展示任务,则调用任务面板的悬停事件处理方法
        if (QuestBillboard is not null)
        {
            QuestBillboard.performHoverAction(x, y);
            return;
        }

        // 清空悬浮标题和悬浮文本
        hoverTitle = "";
        hoverText = "";
        // 遍历所有的任务选项,判断鼠标是否悬停在某个任务选项上,如果是,则设置悬浮标题和悬浮文本,结束循环
        foreach (var option in QuestNotes.Where(option => option.containsPoint(x, y)))
        {
            hoverTitle = QuestDataDictionary[option.myID].Quest.questTitle;
            hoverText = QuestDataDictionary[option.myID].Quest.currentObjective;
            break;
        }
    }

    /// <summary>处理鼠标左键点击</summary>
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // 如果当前没有展示任务面板,则处理OrderBillboard的鼠标左键点击事件
        if (QuestBillboard is null)
        {
            // 遍历所有的任务选项,判断鼠标是否点击在某个任务选项上
            foreach (var option in QuestNotes.Where(option => option.containsPoint(x, y)))
            {
                if (QuestDataDictionary[option.myID].Acceptable)
                {
                    Game1.netWorldState.Value.SetQuestOfTheDay(QuestDataDictionary[option.myID].Quest);
                    ShowingQuest = option.myID;
                    QuestBillboard = new Billboard(true);
                }

                return;
            }

            // 如果点击在右上角的关闭按钮上,则关闭面板
            if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound(closeSound);
                exitThisMenu();
            }
        }
        else
        {
            QuestBillboard.receiveLeftClick(x, y, playSound);
        }
    }

    public override bool readyToClose()
    {
        if (QuestBillboard is not null)
        {
            QuestBillboard = null;
            return false;
        }

        return true;
    }

    public override void applyMovementKey(int direction)
    {
        if (QuestBillboard is null)
        {
            base.applyMovementKey(direction);
        }
        else
        {
            QuestBillboard.applyMovementKey(direction);
        }
    }

    public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
    {
        if (QuestBillboard is null)
        {
            base.automaticSnapBehavior(direction, oldRegion, oldID);
        }
        else
        {
            QuestBillboard.automaticSnapBehavior(direction, oldRegion, oldID);
        }
    }

    public override void snapToDefaultClickableComponent()
    {
        if (QuestBillboard is null)
        {
            base.snapToDefaultClickableComponent();
            currentlySnappedComponent = getComponentWithID(OptionIndex);
            snapCursorToCurrentSnappedComponent();
        }
        else
        {
            QuestBillboard.snapToDefaultClickableComponent();
        }
    }

    /// <summary>绘制多任务面板</summary>
    public override void draw(SpriteBatch spriteBatch)
    {
        // 如果有正在展示的任务面板,则调用任务面板的绘制方法
        if (QuestBillboard is not null)
        {
            QuestBillboard.draw(spriteBatch);
            return;
        }

        // 绘制阴影
        DrawHelper.DrawShadow();

        // 绘制面板纹理
        spriteBatch.Draw(billboardTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 338, 198), Color.White,
            0f,
            Vector2.Zero, 4f, SpriteEffects.None, 1f);

        // 如果没有任务选项,则绘制"没有任务"的文本
        if (!QuestNotes.Any())
        {
            spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"),
                new Vector2(xPositionOnScreen + 384, yPositionOnScreen + 320), Game1.textColor);
        }
        else
        {
            // 遍历所有的任务选项,绘制任务选项
            foreach (var option in QuestNotes)
            {
                option.draw(spriteBatch, QuestDataDictionary[option.myID].PadColor, 1);
                spriteBatch.Draw(QuestDataDictionary[option.myID].PinTexture, option.bounds, QuestDataDictionary[option.myID].IconSource,
                    QuestDataDictionary[option.myID].PinColor);
                // 如果任务有图标,则绘制任务图标
                if (QuestDataDictionary[option.myID].Icon is not null)
                {
                    spriteBatch.Draw(QuestDataDictionary[option.myID].Icon,
                        new Vector2(option.bounds.X + QuestDataDictionary[option.myID].IconOffset.X,
                            option.bounds.Y + QuestDataDictionary[option.myID].IconOffset.Y),
                        QuestDataDictionary[option.myID].IconSource, QuestDataDictionary[option.myID].IconColor, 0, Vector2.Zero,
                        QuestDataDictionary[option.myID].IconScale,
                        SpriteEffects.FlipHorizontally, 1);
                }
            }
        }

        // 绘制右上角的关闭按钮
        if (upperRightCloseButton != null && shouldDrawCloseButton())
        {
            upperRightCloseButton.draw(spriteBatch);
        }

        // 如果有悬浮文本,则绘制悬浮文本
        if (hoverText.Length > 0)
        {
            drawHoverText(spriteBatch, hoverText, Game1.smallFont, 0, 0, -1, (hoverTitle.Length > 0) ? hoverTitle : null);
        }

        // 绘制鼠标
        drawMouse(spriteBatch);
    }

    /// <summary>根据宽度和高度,获取一个没有被其他任务占用的矩形区域,该区域用于放置新的任务</summary>
    private Rectangle? GetFreeBounds(int width1, int height1)
    {
        // 如果宽度和高度大于面板的宽度和高度,则输出错误警告
        if (width1 >= boardRect.Width || height1 >= boardRect.Height)
        {
            ModEntry.SMonitor.Log($"note size {width1},{height1} is too big for the screen", LogLevel.Warn);
            return null;
        }

        // 设置最大尝试次数为10000
        var tries = 10000;
        while (tries > 0)
        {
            // 随机生成一个矩形区域
            var rectangle = new Rectangle(xPositionOnScreen + Game1.random.Next(boardRect.X, boardRect.Right - width1),
                yPositionOnScreen + Game1.random.Next(boardRect.Y, boardRect.Bottom - height1), width1, height1);
            // 遍历所有的可点击组件,计算是否有碰撞发生
            var collision = QuestNotes.Any(cc =>
                Math.Abs(cc.bounds.Center.X - rectangle.Center.X) < rectangle.Width * ModEntry.Config.XOverlapBoundary ||
                Math.Abs(cc.bounds.Center.Y - rectangle.Center.Y) < rectangle.Height * ModEntry.Config.YOverlapBoundary);
            // 如果碰撞发生,则尝试次数减1,否则返回矩形区域
            if (collision)
                tries--;
            else
                return rectangle;
        }

        // 如果尝试次数用完,还没有获得不冲突的矩形区域,则放回null
        return null;
    }
}