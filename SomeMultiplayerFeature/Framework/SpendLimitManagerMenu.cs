using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class SpendLimitManagerMenu : IClickableMenu
{
    private const int ModelsPerPage = 10;

    private int currentPage;
    private readonly ClickableTextureComponent upArrow;
    private readonly ClickableTextureComponent downArrow;
    private readonly ClickableTextureComponent increaseButton;
    private readonly ClickableTextureComponent decreaseButton;
    private readonly List<SpendLimitModel> spendLimitModels = new();
    private readonly List<ClickableComponent> spendLimitModelSlots = new();



    public SpendLimitManagerMenu()
        : base(Game1.uiViewport.Width / 2 - 304, Game1.uiViewport.Height / 2 - 408, 608, 816)
    {
        this.upArrow = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen -24, 44, 48),
            Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
        this.downArrow = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height -24, 44, 48),
            Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
        this.increaseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 36, this.yPositionOnScreen - 40, 28, 32),
            Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f);
        this.decreaseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen - 40, 28, 32),
            Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f);
        for (var i = 0; i < ModelsPerPage; i++) this.spendLimitModelSlots.Add(new ClickableComponent(this.GetSlotRectangle(i), ""));

        this.InitSpendLimitModels();
    }

    public override void draw(SpriteBatch b)
    {
        drawTextureBox(b, this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White);

        for (var i = 0; i < ModelsPerPage; i++)
        {
            var targetIndex = i + ModelsPerPage * this.currentPage;
            if (targetIndex < this.spendLimitModels.Count)
            {
                this.spendLimitModels[targetIndex].Draw(b);
            }
        }

        this.upArrow.draw(b);
        this.downArrow.draw(b);
        this.increaseButton.draw(b);
        this.decreaseButton.draw(b);

        this.drawMouse(b);
    }

    public override void performHoverAction(int x, int y)
    {
        this.upArrow.tryHover(x, y);
        this.downArrow.tryHover(x, y);
        this.increaseButton.tryHover(x, y);
        this.decreaseButton.tryHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        for (var i = 0; i < ModelsPerPage; i++)
        {
            var slot = this.spendLimitModelSlots[i];
            if (slot.containsPoint(x, y))
            {
                var targetIndex = i + ModelsPerPage * this.currentPage;
                if (targetIndex < this.spendLimitModels.Count)
                {
                    this.spendLimitModels[targetIndex].ReceiveLeftClick(x, y);
                }
            }
        }

        if (this.upArrow.containsPoint(x, y))
        {
            this.currentPage--;
        }
        else if (this.downArrow.containsPoint(x, y))
        {
            this.currentPage++;
        }
        else if (this.increaseButton.containsPoint(x, y))
        {
            foreach (var model in this.spendLimitModels) model.ChangeFarmerSpendLimit(true, out _);
            Log.NoIconHUDMessage("已将所有玩家的额度增加1000金", 500f);
        }
        else if (this.decreaseButton.containsPoint(x, y))
        {
            foreach (var model in this.spendLimitModels) model.ChangeFarmerSpendLimit(false, out _);
            Log.NoIconHUDMessage("已将所有玩家的额度减少1000金", 500f);
        }

        this.currentPage = Math.Max(0, Math.Min(this.currentPage, this.spendLimitModels.Count / ModelsPerPage));
        this.SetModelsPosition();
    }

    private void InitSpendLimitModels()
    {
        foreach (var farmer in Game1.getAllFarmhands().Where(x => !x.isUnclaimedFarmhand))
            this.spendLimitModels.Add(new SpendLimitModel(farmer));
        this.SetModelsPosition();
    }

    private void SetModelsPosition()
    {
        for (var i = 0; i < ModelsPerPage; i++)
        {
            var targetIndex = i + ModelsPerPage * this.currentPage;
            if (targetIndex < this.spendLimitModels.Count)
            {
                var modelBounds = this.GetSlotRectangle(i);
                this.spendLimitModels[targetIndex].SetPosition(modelBounds.X, modelBounds.Y);
            }
        }
    }

    private Rectangle GetSlotRectangle(int index)
    {
        return new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 16 + 80 * index, 576, 64);
    }
}