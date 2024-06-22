using BetterCabin.Framework.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;

namespace BetterCabin.Framework.UI;

internal abstract class Box
{
    private readonly Building building;
    protected readonly Cabin Cabin;
    protected readonly ModConfig Config;

    private SpriteFont Font => Game1.smallFont;
    protected abstract Color TextColor { get; }
    protected abstract string Text { get; }
    private Point Size => Font.MeasureString(Text).ToPoint() + new Point(32, 32);
    private Point Position
    {
        get
        {
            var buildingPosition = new Point(building.tileX.Value * 64 - Game1.viewport.X, building.tileY.Value * 64 - Game1.viewport.Y);
            return new Point(buildingPosition.X - Size.X / 2 + Offset.X, buildingPosition.Y - Size.Y / 2 + Offset.Y);
        }
    }
    protected abstract Point Offset { get; }

    protected Box(Building building, Cabin cabin, ModConfig config)
    {
        this.building = building;
        Cabin = cabin;
        Config = config;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, Position.X, Position.Y, Size.X, Size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, Text, Font, new Vector2(Position.X + 16, Position.Y + 16), TextColor, 1f, 1f);
    }
}