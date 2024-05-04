using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;

namespace HelpWanted.Framework;

public sealed class HWQuestBoard : Billboard
{
    private readonly ModConfig config;

    private static readonly List<ClickableTextureComponent> QuestNotes = new();
    private static readonly Dictionary<int, QuestData> QuestDataDictionary = new();
    private static Rectangle boardRect = new(70 * 4, 52 * 4, 196 * 4, 119 * 4);
    private const int OptionIndex = -4200;

    // 面板纹理
    private readonly Texture2D billboardTexture;

    // 正在展示的任务的ID
    private static int showingQuestID;

    // 正在展示的任务
    private static Quest? showingQuest;

    // 悬浮标题和悬浮文本
    private string hoverTitle = "";
    private string hoverText = "";

    public HWQuestBoard(ModConfig config) : base(true)
    {
        this.config = config;
        
        // 设置面板纹理
        billboardTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites/Billboard");
        acceptQuestButton.visible = true;
        showingQuest = null;
        if (ModEntry.QuestList.Count > 0)
        {
            // 清空任务选项列表和任务数据字典
            QuestNotes.Clear();
            QuestDataDictionary.Clear();
            // 遍历所有的任务数据,创建任务选项
            var questList = ModEntry.QuestList;
            for (var i = 0; i < questList.Count; i++)
            {
                var size = new Point(
                    (int)(questList[i].PadTextureSource.Width * config.NoteScale),
                    (int)(questList[i].PadTextureSource.Height * config.NoteScale));
                var bounds = GetFreeBounds(size.X, size.Y);
                if (bounds is null) break;
                QuestNotes.Add(new ClickableTextureComponent(bounds.Value,
                    questList[i].PadTexture, questList[i].PadTextureSource, config.NoteScale)
                {
                    // 设置该选项的ID
                    myID = OptionIndex - i,
                    // // 如果该选项是最左侧的选项,则左邻居ID为-1,否则为当前选项ID+1
                    // leftNeighborID = i > 0 ? OptionIndex - i + 1 : -1,
                    // // 如果该选项是最右侧的选项,则右邻居ID为-1,否则为当前选项ID-1
                    // rightNeighborID = i < questList.Count - 1 ? OptionIndex - i - 1 : -1
                });
                QuestDataDictionary[QuestNotes[i].myID] = questList[i];
            }

            ModEntry.QuestList.Clear();
        }
    }

    /// <summary>处理鼠标悬停事件</summary>
    public override void performHoverAction(int x, int y)
    {
        // 关闭按钮逻辑
        upperRightCloseButton?.tryHover(x, y, 0.5f);
        
        if (showingQuest is null)
        {
            // 任务便签逻辑
            hoverTitle = "";
            hoverText = "";
            foreach (var option in QuestNotes.Where(option => option.containsPoint(x, y)))
            {
                hoverTitle = QuestDataDictionary[option.myID].Quest.questTitle;
                hoverText = QuestDataDictionary[option.myID].Quest.currentObjective;
                break;
            }
        }
        else
        {
            // 接受任务按钮逻辑
            hoverTitle = "";
            hoverText = "";
            var oldScale = acceptQuestButton.scale;
            acceptQuestButton.scale = acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
            if (acceptQuestButton.scale > oldScale) Game1.playSound("Cowboy_gunshot");
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // 如果当前没有展示任务面板,则处理OrderBillboard的鼠标左键点击事件
        if (showingQuest is null)
        {
            // 关闭按钮逻辑
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound(closeSound);
                exitThisMenu();
            }

            // 任务便签逻辑
            foreach (var option in QuestNotes.Where(option => option.containsPoint(x, y)))
            {
                showingQuestID = option.myID;
                showingQuest = QuestDataDictionary[option.myID].Quest;
                return;
            }
        }
        else
        {
            // 关闭按钮逻辑
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound(closeSound);
                showingQuest = null;
                return;
            }

            // 接受任务按钮逻辑
            if (acceptQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                showingQuest.dayQuestAccepted.Value = Game1.Date.TotalDays;
                Game1.player.questLog.Add(showingQuest);
                QuestDataDictionary.Remove(showingQuestID);
                QuestNotes.RemoveAll(option => option.myID == showingQuestID);
                showingQuest = null;
            }
        }
    }

    /// <summary>绘制多任务面板</summary>
    public override void draw(SpriteBatch spriteBatch)
    {
        // var hideMouse = false;
        
        // 绘制阴影
        if (!Game1.options.showClearBackgrounds) spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        
        // 绘制面板纹理
        spriteBatch.Draw(billboardTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 338, 198), Color.White,
            0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

        // if (Game1.options.SnappyMenus) hideMouse = true;

        if (!QuestNotes.Any())
        {
            spriteBatch.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"),
                new Vector2(xPositionOnScreen + 384, yPositionOnScreen + 320), Game1.textColor);
        }
        else
        {
            if (showingQuest is null)
                DrawQuestNotes(spriteBatch);
            else
            {
                // hideMouse = false;
                DrawShowingQuest(spriteBatch);
            }
        }

        // 绘制星星
        for (var i = 0; i < Game1.stats.Get("BillboardQuestsDone") % 3; i++)
            spriteBatch.Draw(billboardTexture, Position + new Vector2(18 + 12 * i, 36f) * 4f, new Rectangle(140, 397, 10, 11), Color.White,
                0f, Vector2.Zero, 4f, SpriteEffects.None, 0.6f);
        
        // 绘制祝尼魔
        if (Game1.player.hasCompletedCommunityCenter())
            spriteBatch.Draw(billboardTexture, Position + new Vector2(290f, 59f) * 4f, new Rectangle(0, 427, 39, 54), Color.White, 0f,
                Vector2.Zero, 4f, SpriteEffects.None, 0.6f);

        // 绘制右上角的关闭按钮
        if (upperRightCloseButton != null && shouldDrawCloseButton()) upperRightCloseButton.draw(spriteBatch);

        // if (hideMouse) return;
        
        // 绘制鼠标
        Game1.mouseCursorTransparency = 1f;
        drawMouse(spriteBatch);
        
        // 绘制悬浮文本
        if (hoverText.Length > 0) drawHoverText(spriteBatch, hoverText, Game1.smallFont, 0, 0, -1, hoverTitle);
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
                Math.Abs(cc.bounds.Center.X - rectangle.Center.X) < rectangle.Width * config.XOverlapBoundary ||
                Math.Abs(cc.bounds.Center.Y - rectangle.Center.Y) < rectangle.Height * config.YOverlapBoundary);
            // 如果碰撞发生,则尝试次数减1,否则返回矩形区域
            if (collision)
                tries--;
            else
                return rectangle;
        }

        // 如果尝试次数用完,还没有获得不冲突的矩形区域,则放回null
        return null;
    }
    
    private void DrawShowingQuest(SpriteBatch spriteBatch)
    {
        var font = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
        var description = Game1.parseText(showingQuest!.questDescription, font, 640);
        // 绘制任务描述
        Utility.drawTextWithShadow(spriteBatch, description, font, new Vector2(xPositionOnScreen + 320 + 32, yPositionOnScreen + 256), Game1.textColor, 1f, -1f,
            -1, -1, 0.5f);
        // 绘制接受任务按钮
        drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), acceptQuestButton.bounds.X, acceptQuestButton.bounds.Y,
            acceptQuestButton.bounds.Width, acceptQuestButton.bounds.Height, acceptQuestButton.scale > 1f ? Color.LightPink : Color.White, 4f * acceptQuestButton.scale);
        Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont,
            new Vector2(acceptQuestButton.bounds.X + 12, acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
        if (Game1.stats.Get("BillboardQuestsDone") % 3 != 2) return;
        // 绘制奖券
        Utility.drawWithShadow(spriteBatch, Game1.content.Load<Texture2D>("TileSheets\\Objects_2"), Position + new Vector2(215f, 144f) * 4f,
            new Rectangle(80, 128, 16, 16), Color.White, 0f, Vector2.Zero, 4f);
        SpriteText.drawString(spriteBatch, "x1", (int)Position.X + 936, (int)Position.Y + 596);
    }
    
    private void DrawQuestNotes(SpriteBatch spriteBatch)
    {
        // 遍历所有的任务选项,绘制任务选项
        foreach (var option in QuestNotes)
        {
            var questData = QuestDataDictionary[option.myID];
            // 绘制 Pad
            option.draw(spriteBatch, questData.PadColor, 1);
            // 绘制 Pin
            spriteBatch.Draw(questData.PinTexture, option.bounds, questData.PinTextureSource, questData.PinColor);
            // 绘制 Icon
            spriteBatch.Draw(questData.Icon,
                new Vector2(option.bounds.X + questData.IconOffset.X, option.bounds.Y + questData.IconOffset.Y),
                questData.IconSource, questData.IconColor, 0, Vector2.Zero, questData.IconScale, SpriteEffects.FlipHorizontally, 1);
        }
    }
}