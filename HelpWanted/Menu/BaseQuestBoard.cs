using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.HelpWanted.Model;

namespace weizinai.StardewValleyMod.HelpWanted.Menu;

public abstract class BaseQuestBoard : IClickableMenu
{
    private const int OptionIndex = -4200;

    private string hoverTitle = "";
    private string hoverText = "";
    private readonly ClickableComponent acceptQuestButton;
    private readonly Texture2D billboardTexture;
    private readonly Rectangle billboardTextureSourceRect;
    private Rectangle boardRect = new(78 * 4, 52 * 4, 184 * 4, 102 * 4);

    private readonly BoardType boardType;
    private int showingQuestId;
    protected Quest? ShowingQuest;

    [MemberNotNullWhen(true, nameof(ShowingQuest))]
    private bool IsShowingQuest => this.ShowingQuest != null;

    public static readonly Dictionary<BoardType, List<QuestNote>> AllQuestNotes = new()
    {
        [BoardType.Vanilla] = new List<QuestNote>(),
        [BoardType.RSV] = new List<QuestNote>()
    };

    private List<QuestNote> CurrentQuestNotes => AllQuestNotes[this.boardType];

    protected BaseQuestBoard(BoardType boardType, Texture2D billboardTexture, Rectangle billboardTextureSourceRect) : base(0, 0, 0, 0, true)
    {
        // 位置和大小逻辑
        this.width = 338 * 4;
        this.height = 198 * 4;
        var center = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
        this.xPositionOnScreen = (int)center.X;
        this.yPositionOnScreen = (int)center.Y;

        // 背景逻辑
        this.boardType = boardType;
        this.billboardTexture = billboardTexture;
        this.billboardTextureSourceRect = billboardTextureSourceRect;

        // 接受任务按钮逻辑
        var stringSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));
        this.acceptQuestButton = new ClickableComponent(
            new Rectangle(
                this.xPositionOnScreen + this.width / 2 - 128,
                this.yPositionOnScreen + this.height - 128,
                (int)stringSize.X + 24,
                (int)stringSize.Y + 24
            ),
            ""
        );

        // 关闭按钮逻辑
        this.upperRightCloseButton = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48, 48),
            Game1.mouseCursors,
            new Rectangle(337, 494, 12, 12),
            4f
        );

        // 初始化
        this.InitQuestNotes();
    }

    public override void performHoverAction(int x, int y)
    {
        // 关闭按钮逻辑
        this.upperRightCloseButton?.tryHover(x, y, 0.5f);

        if (this.IsShowingQuest)
        {
            // 接受任务按钮逻辑
            var oldScale = this.acceptQuestButton.scale;
            this.acceptQuestButton.scale = this.acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
            if (this.acceptQuestButton.scale > oldScale) Game1.playSound("Cowboy_gunshot");
        }
        else
        {
            // 任务便签逻辑
            this.hoverTitle = "";
            this.hoverText = "";

            foreach (var option in this.CurrentQuestNotes.Where(option => option.containsPoint(x, y)))
            {
                this.hoverTitle = option.QuestModel.Quest.questTitle;
                this.hoverText = option.QuestModel.Quest.currentObjective;

                break;
            }
        }
    }

    public override void receiveKeyPress(Keys key)
    {
        if (this.IsShowingQuest && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
            this.CloseShowingQuest();

            return;
        }

        base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.IsShowingQuest)
        {
            // 关闭按钮逻辑
            if (this.upperRightCloseButton != null && this.upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound(this.closeSound);
                this.CloseShowingQuest();

                return;
            }

            // 接受任务按钮逻辑
            if (this.acceptQuestButton.containsPoint(x, y))
            {
                Game1.playSound("newArtifact");
                this.ShowingQuest.dayQuestAccepted.Value = Game1.Date.TotalDays;
                Game1.player.questLog.Add(this.ShowingQuest);
                this.CurrentQuestNotes.RemoveAll(option => option.myID == this.showingQuestId);
                this.allClickableComponents?.RemoveAll(component => component.myID == this.showingQuestId);
                this.CloseShowingQuest();
            }
        }
        else
        {
            // 关闭按钮逻辑
            if (this.upperRightCloseButton != null && this.upperRightCloseButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound(this.closeSound);
                this.exitThisMenu();
            }

            // 任务便签逻辑
            foreach (var option in this.CurrentQuestNotes.Where(option => option.containsPoint(x, y)))
            {
                this.hoverTitle = "";
                this.hoverText = "";
                this.showingQuestId = option.myID;
                this.ShowingQuest = option.QuestModel.Quest;
                this.acceptQuestButton.visible = true;

                return;
            }
        }
    }

    public override void draw(SpriteBatch b)
    {
        // 阴影绘制逻辑
        if (!Game1.options.showClearBackgrounds) b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

        // 面板绘制逻辑
        b.Draw(
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

        if (!this.CurrentQuestNotes.Any())
        {
            b.DrawString(
                Game1.dialogueFont,
                Game1.content.LoadString("Strings\\UI:Billboard_NothingPosted"),
                new Vector2(this.xPositionOnScreen + 384, this.yPositionOnScreen + 320),
                Game1.textColor
            );
        }
        else
        {
            if (this.IsShowingQuest)
                this.DrawShowingQuest(b);
            else
                this.DrawQuestNotes(b);
        }

        if (this.boardType == BoardType.Vanilla)
        {
            // 星星绘制逻辑
            for (var i = 0; i < Game1.stats.Get("BillboardQuestsDone") % 3; i++)
            {
                b.Draw(
                    this.billboardTexture,
                    this.Position + new Vector2(18 + 12 * i, 36f) * 4f,
                    new Rectangle(140, 397, 10, 11),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.6f
                );
            }

            // 祝尼魔绘制逻辑
            if (Game1.player.hasCompletedCommunityCenter())
            {
                b.Draw(
                    this.billboardTexture,
                    this.Position + new Vector2(290f, 59f) * 4f,
                    new Rectangle(0, 427, 39, 54), Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.6f
                );
            }
        }

        // 关闭按钮绘制逻辑
        this.upperRightCloseButton.draw(b);

        // 鼠标绘制逻辑
        Game1.mouseCursorTransparency = 1f;
        this.drawMouse(b);

        // 悬浮文本绘制
        if (this.hoverText.Length > 0)
        {
            drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, this.hoverTitle);
        }
    }

    private void InitQuestNotes()
    {
        var questList = this.boardType == BoardType.Vanilla
            ? VanillaQuestManager.Instance.QuestList
            : RSVQuestManager.Instance.QuestList;

        if (questList.Count <= 0) return;

        for (var i = questList.Count - 1; i >= 0; i--)
        {
            var bounds = this.GetFreeBounds((int)questList[i].NoteWidth, (int)questList[i].NoteHeight);

            if (bounds is null)
            {
                Logger.Debug($"Quest menu capacity reached: {this.CurrentQuestNotes.Count} quests placed, unable to accommodate remaining {questList.Count}");

                break;
            }

            this.CurrentQuestNotes.Add(new QuestNote(questList[i], bounds.Value)
            {
                // 设置该选项的ID
                myID = OptionIndex - i,
            });
            questList.RemoveAt(i);
        }
    }

    private Rectangle? GetFreeBounds(int _width, int _height)
    {
        if (_width >= this.boardRect.Width || _height >= this.boardRect.Height)
        {
            Logger.Error("Note dimensions exceed quest menu boundaries.");

            return null;
        }

        var tries = 1000;

        while (tries > 0)
        {
            var rectangle = new Rectangle(
                this.xPositionOnScreen + ModEntry.Random.Next(this.boardRect.X, this.boardRect.Right - _width),
                this.yPositionOnScreen + ModEntry.Random.Next(this.boardRect.Y, this.boardRect.Bottom - _height),
                _width,
                _height
            );

            var collision = this.CurrentQuestNotes.Any(note =>
                Math.Abs(note.bounds.Center.X - rectangle.Center.X) < rectangle.Width * ModConfig.Instance.XOverlapBoundary
                || Math.Abs(note.bounds.Center.Y - rectangle.Center.Y) < rectangle.Height * ModConfig.Instance.YOverlapBoundary
            );

            if (collision)
                tries--;
            else
                return rectangle;
        }

        return null;
    }

    private void DrawQuestNotes(SpriteBatch b)
    {
        foreach (var questNote in this.CurrentQuestNotes)
        {
            questNote.Draw(b);
        }
    }

    private void DrawShowingQuest(SpriteBatch b)
    {
        var font = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? Game1.smallFont : Game1.dialogueFont;
        // 任务描述逻辑
        Utility.drawTextWithShadow(
            b,
            Game1.parseText(this.ShowingQuest!.questDescription, font, 640),
            font,
            new Vector2(this.xPositionOnScreen + 320 + 32, this.yPositionOnScreen + 256),
            Game1.textColor,
            1f,
            -1f,
            -1,
            -1,
            0.5f
        );
        // 接受任务按钮逻辑
        drawTextureBox(
            b,
            Game1.mouseCursors,
            new Rectangle(403, 373, 9, 9),
            this.acceptQuestButton.bounds.X,
            this.acceptQuestButton.bounds.Y,
            this.acceptQuestButton.bounds.Width,
            this.acceptQuestButton.bounds.Height,
            this.acceptQuestButton.scale > 1f ? Color.LightPink : Color.White,
            4f * this.acceptQuestButton.scale
        );
        Utility.drawTextWithShadow(
            b,
            Game1.content.LoadString("Strings\\UI:AcceptQuest"),
            Game1.dialogueFont,
            new Vector2(this.acceptQuestButton.bounds.X + 12, this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)),
            Game1.textColor
        );

        // 奖券逻辑
        if (this.boardType == BoardType.Vanilla)
        {
            if (Game1.stats.Get("BillboardQuestsDone") % 3 != 2) return;

            Utility.drawWithShadow(
                b,
                Game1.content.Load<Texture2D>("TileSheets\\Objects_2"),
                this.Position + new Vector2(215f, 144f) * 4f,
                new Rectangle(80, 128, 16, 16),
                Color.White,
                0f,
                Vector2.Zero,
                4f
            );
            SpriteText.drawString(b, "x1", (int)this.Position.X + 936, (int)this.Position.Y + 596);
        }
    }

    private void CloseShowingQuest()
    {
        this.ShowingQuest = null;
        this.showingQuestId = -1;
        this.acceptQuestButton.visible = false;
    }

    public override void populateClickableComponentList()
    {
        this.allClickableComponents = new List<ClickableComponent> { this.upperRightCloseButton };
        this.allClickableComponents.AddRange(this.CurrentQuestNotes);
    }

    public override void applyMovementKey(int direction)
    {
        if (this.allClickableComponents == null)
        {
            this.populateClickableComponentList();
        }

        this.MoveCursorInDirection(direction);
    }

    private void MoveCursorInDirection(int direction)
    {
        if (this.currentlySnappedComponent == null)
        {
            this.snapToDefaultClickableComponent();
        }

        if (this.IsShowingQuest)
        {
            this.ToggleQuestButtons();
        }
        else
        {
            this.NavigateToNextComponent(direction);
        }
    }

    private void ToggleQuestButtons()
    {
        this.currentlySnappedComponent = this.currentlySnappedComponent == this.acceptQuestButton
            ? this.upperRightCloseButton
            : this.acceptQuestButton;

        this.snapCursorToCurrentSnappedComponent();
        Game1.playSound("toolSwap");
    }

    public override void snapToDefaultClickableComponent()
    {
        this.currentlySnappedComponent = this.IsShowingQuest
            ? this.acceptQuestButton
            : this.CurrentQuestNotes.FirstOrDefault() ?? (ClickableComponent)this.upperRightCloseButton;
        this.snapCursorToCurrentSnappedComponent();
    }

    private void NavigateToNextComponent(int direction)
    {
        var nextComponent = this.FindNextComponent(direction);

        if (nextComponent != null)
        {
            this.currentlySnappedComponent = nextComponent;
            this.SnapCursorToCurrentNote();
            Game1.playSound("toolSwap");
        }
    }

    private ClickableComponent? FindNextComponent(int direction)
    {
        if (this.allClickableComponents == null || this.currentlySnappedComponent == null) return null;

        var currentX = this.currentlySnappedComponent.bounds.X;
        var currentY = this.currentlySnappedComponent.bounds.Y;

        var candidates = direction switch
        {
            0 => this.allClickableComponents.Where(c => c.bounds.Y < currentY),
            1 => this.allClickableComponents.Where(c => c.bounds.X > currentX),
            2 => this.allClickableComponents.Where(c => c.bounds.Y > currentY),
            3 => this.allClickableComponents.Where(c => c.bounds.X < currentX),
            _ => Enumerable.Empty<ClickableComponent>()
        };

        return candidates
            .OrderBy(c => GetDistance(c, currentX, currentY, direction))
            .FirstOrDefault();
    }

    private static int GetDistance(ClickableComponent c, int currentX, int currentY, int direction)
    {
        return direction switch
        {
            0 => currentY - c.bounds.Center.Y,
            1 => c.bounds.Center.X - currentX,
            2 => c.bounds.Center.Y - currentY,
            3 => currentX - c.bounds.Center.X,
            _ => 0,
        };
    }

    private void SnapCursorToCurrentNote()
    {
        if (this.currentlySnappedComponent != null)
        {
            Game1.setMousePosition(
                this.currentlySnappedComponent.bounds.Center.X,
                this.currentlySnappedComponent.bounds.Bottom - this.currentlySnappedComponent.bounds.Height / 15,
                true
            );
        }
    }

    public static void ClearCache()
    {
        foreach (var notes in AllQuestNotes.Values)
        {
            notes.Clear();
        }
    }
}