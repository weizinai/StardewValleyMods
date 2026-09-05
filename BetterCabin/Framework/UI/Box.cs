using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.UI;

internal abstract class Box
{
    private readonly Building building;
    protected readonly Cabin cabin;
    protected readonly ModConfig config;

    private SpriteFont font => Game1.smallFont;
    protected abstract Color textColor { get; }
    protected abstract string text { get; }
    private Point size => this.font.MeasureString(this.text).ToPoint() + new Point(32, 32);

    private Point position
    {
        get
        {
            var buildingPosition = new Point(this.building.tileX.Value * 64 - Game1.viewport.X, this.building.tileY.Value * 64 - Game1.viewport.Y);

            return new Point(buildingPosition.X - this.size.X / 2 + this.offset.X, buildingPosition.Y - this.size.Y / 2 + this.offset.Y);
        }
    }

    protected abstract Point offset { get; }

    protected Box(Building building, Cabin cabin, ModConfig config)
    {
        this.building = building;
        this.cabin = cabin;
        this.config = config;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, this.position.X, this.position.Y, this.size.X, this.size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, this.text, this.font, new Vector2(this.position.X + 16, this.position.Y + 16), this.textColor, 1f, 1f);
    }
}
