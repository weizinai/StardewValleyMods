using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

internal class SpendLimitModel
{
    private Vector2 position;
    private Rectangle Bounds => new((int)this.position.X, (int)this.position.Y, 576, 64);
    private Rectangle TextBoxBounds => new(this.textBox.X, this.textBox.Y, this.textBox.Width, this.textBox.Height);

    private ClickableTextureComponent increaseButton = null!;
    private StardewValley.Menus.TextBox textBox = null!;
    private ClickableTextureComponent decreaseButton = null!;

    private readonly Farmer farmer;

    public SpendLimitModel(Farmer farmer)
    {
        this.farmer = farmer;
        this.InitComponent();
    }

    public void ReceiveLeftClick(int x, int y)
    {
        if (this.increaseButton.containsPoint(x, y))
        {
            this.ChangeFarmerSpendLimit(true, out var newLimit);
            Log.NoIconHUDMessage($"已将{this.farmer.Name}的额度设置为{newLimit}元", 500f);
        }
        else if (this.decreaseButton.containsPoint(x, y))
        {
            this.ChangeFarmerSpendLimit(false, out var newLimit);
            Log.NoIconHUDMessage($"已将{this.farmer.Name}的额度设置为{newLimit}元", 500f);
        }
        else if (this.TextBoxBounds.Contains(x, y))
        {
            this.textBox.Selected = true;
        }
    }

    public void Draw(SpriteBatch b)
    {
        this.farmer.FarmerRenderer.drawMiniPortrat(b, this.position, 0f, 4f, 2, this.farmer);
        b.DrawString(Game1.smallFont, this.farmer.Name, this.position + new Vector2(64, 20), Game1.textColor);

        this.increaseButton.draw(b);
        this.decreaseButton.draw(b);
        this.textBox.Draw(b, false);
    }

    private void InitComponent()
    {
        this.increaseButton = new ClickableTextureComponent(new Rectangle(0, 0, 28, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f);
        this.textBox = new StardewValley.Menus.TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
        {
            Width = 256
        };
        this.decreaseButton = new ClickableTextureComponent(new Rectangle(0, 0, 28, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f);

        this.SetTextBoxContent();
        this.textBox.OnEnterPressed += textbox =>
        {
            if (int.TryParse(textbox.Text, out var value))
            {
                SpendLimitHelper.SetFarmerSpendLimit(this.farmer.Name, value);
                Log.NoIconHUDMessage($"已将{this.farmer.Name}的额度设置为{value}元", 500f);
            }
            else
            {
                this.SetTextBoxContent();
                Log.ErrorHUDMessage("输入内容错误");
            }
        };
    }

    public void SetPosition(int x, int y)
    {
        this.position = new Vector2(x, y);
        this.increaseButton.setPosition(this.Bounds.Right - 28, this.Bounds.Y + 16);
        this.textBox.X = this.increaseButton.bounds.X - 264;
        this.textBox.Y = this.Bounds.Y + 8;
        this.decreaseButton.setPosition(this.textBox.X - 36, this.Bounds.Y + 16);
    }

    private void SetTextBoxContent()
    {
        SpendLimitHelper.TryGetFarmerSpendLimit(this.farmer.Name, out var value);
        this.textBox.Text = value.ToString();
    }

    public void ChangeFarmerSpendLimit(bool isIncrease, out int newLimit)
    {
        SpendLimitHelper.TryGetFarmerSpendLimit(this.farmer.Name, out var oldLimit);
        newLimit = isIncrease ? oldLimit + 1000 : Math.Max(0, oldLimit - 1000);
        SpendLimitHelper.SetFarmerSpendLimit(this.farmer.Name, newLimit);
        this.SetTextBoxContent();
    }
}