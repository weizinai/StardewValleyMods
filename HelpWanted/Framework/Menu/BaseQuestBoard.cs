using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace weizinai.StardewValleyMod.HelpWanted.Framework.Menu
{
    internal abstract class BaseQuestBoard : IClickableMenu
    {
        protected readonly ClickableComponent AcceptQuestButton;
        protected string HoverTitle = "";
        protected string HoverText = "";
        protected int ShowingQuestId;
        protected Quest? ShowingQuest;

        protected bool IsShowingQuest => ShowingQuest is not null;

        protected Rectangle BoardRect = new(78 * 4, 52 * 4, 184 * 4, 102 * 4);
        protected const int OptionIndex = -4200;

        // Construtor
        protected BaseQuestBoard()
            : base(0, 0, 0, 0, true)
        {
            // 位置和大小逻辑
            this.width = 338 * 4;
            this.height = 198 * 4;
            var center = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height);
            this.xPositionOnScreen = (int)center.X;
            this.yPositionOnScreen = (int)center.Y;

            // 接受任务按钮逻辑
            var stringSize = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest"));
            this.AcceptQuestButton = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - 128, this.yPositionOnScreen + this.height - 128,
                (int)stringSize.X + 24, (int)stringSize.Y + 24), "");

            // 关闭按钮逻辑
            this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 20, this.yPositionOnScreen, 48, 48),
                Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
        }


        public override void receiveKeyPress(Keys key)
        {
            if (IsExitButtonPressed(key) && IsShowingQuest)
            {
                CloseQuestView();
                return;
            }

            base.receiveKeyPress(key);
        }

        private bool IsExitButtonPressed(Keys key)
        {
            return Game1.options.doesInputListContain(Game1.options.menuButton, key)
                || Game1.options.doesInputListContain(Game1.options.cancelButton, key);
        }

        protected void CloseQuestView()
        {
            ShowingQuest = null;
            ShowingQuestId = -1;
            AcceptQuestButton.visible = false;
        }

        public override void applyMovementKey(int direction)
        {
            if (allClickableComponents == null)
            {
                populateClickableComponentList();
            }

            moveCursorInDirection(direction);
        }

        public new void moveCursorInDirection(int direction)
        {
            if (IsShowingQuest)
            {
                ToggleQuestButtons();
                return;
            }

            if (currentlySnappedComponent == null)
            {
                SnapToDefaultComponent();
            }

            NavigateToNextComponent(direction);
        }

        private void ToggleQuestButtons()
        {
            currentlySnappedComponent = (currentlySnappedComponent == AcceptQuestButton)
                ? upperRightCloseButton
                : AcceptQuestButton;

            snapCursorToCurrentSnappedComponent();
            Game1.playSound("toolSwap");
        }

        private void SnapToDefaultComponent()
        {
            var list = allClickableComponents;
            if (list != null && list.Count > 0)
            {
                snapToDefaultClickableComponent();
                currentlySnappedComponent ??= allClickableComponents[0];
            }
        }

        private void NavigateToNextComponent(int direction)
        {
            var nextComponent = FindNextComponent(direction);
            if (nextComponent != null)
            {
                currentlySnappedComponent = nextComponent;
                snapCursorToCurrentNote();
                Game1.playSound("toolSwap");
            }
        }

        private ClickableComponent? FindNextComponent(int direction)
        {
            if (allClickableComponents == null || currentlySnappedComponent == null)
                return null;

            int currentX = currentlySnappedComponent.bounds.X;
            int currentY = currentlySnappedComponent.bounds.Y;

            var candidates = direction switch
            {
                0 => allClickableComponents.Where(c => c.bounds.Y < currentY),
                1 => allClickableComponents.Where(c => c.bounds.X > currentX),
                2 => allClickableComponents.Where(c => c.bounds.Y > currentY),
                3 => allClickableComponents.Where(c => c.bounds.X < currentX),
                _ => Enumerable.Empty<ClickableComponent>(),
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

        private void snapCursorToCurrentNote()
        {
            if (currentlySnappedComponent != null)
            {
                Game1.setMousePosition(
                    currentlySnappedComponent.bounds.Center.X,
                    currentlySnappedComponent.bounds.Bottom - currentlySnappedComponent.bounds.Height / 15,
                    ui_scale: true
                );
            }
        }
    }
}
