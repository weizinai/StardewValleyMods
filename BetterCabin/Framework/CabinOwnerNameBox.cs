using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

namespace BetterCabin.Framework;

internal class CabinOwnerNameBox
{
    private readonly Building building;
    private readonly Cabin cabin;

    private SpriteFont Font => Game1.smallFont;
    private string Name => cabin.owner.displayName;
    private Point Size => Font.MeasureString(Name).ToPoint() + new Point(32, 32);
    private Point Position => new(building.tileX.Value * 64 - Game1.viewport.X, building.tileY.Value * 64 - Game1.viewport.Y);

    public CabinOwnerNameBox(Building building, Cabin cabin)
    {
        this.building = building;
        this.cabin = cabin;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, Position.X, Position.Y, Size.X, Size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, Name, Font, new Vector2(Position.X + 16, Position.Y + 16), 
            Game1.player.Equals(cabin.owner) ? Color.Red : Color.Black, 1f, 1f);
    }
}