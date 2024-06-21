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
    private readonly Point offset;

    private SpriteFont Font => Game1.smallFont;
    private string Name => cabin.owner.displayName;
    private Point Size => Font.MeasureString(Name).ToPoint() + new Point(32, 32);

    private Point Position
    {
        get
        {
            var buildingPosition = new Point(building.tileX.Value * 64 - Game1.viewport.X, building.tileY.Value * 64 - Game1.viewport.Y);
            return new Point(buildingPosition.X - Size.X / 2 + offset.X, buildingPosition.Y - Size.Y / 2 + offset.Y);
        }
    }

    public CabinOwnerNameBox(Building building, Cabin cabin, Point offset)
    {
        this.building = building;
        this.cabin = cabin;
        this.offset = offset;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, Position.X, Position.Y, Size.X, Size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, Name, Font, new Vector2(Position.X + 16, Position.Y + 16),
            Game1.player.Equals(cabin.owner) ? Color.Red : Color.Black, 1f, 1f);
    }
}