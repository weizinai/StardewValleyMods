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
    private readonly ModConfig config;

    private SpriteFont Font => Game1.smallFont;
    private Color TextColor
    {
        get
        {
            if (Game1.player.Equals(cabin.owner))
            {
                return config.OwnerColor;
            }

            return Game1.player.team.playerIsOnline(cabin.owner.UniqueMultiplayerID) ? config.OnlineFarmerColor : config.OfflineFarmerColor;
        }
    }
    private string Name => cabin.owner.displayName;
    private Point Size => Font.MeasureString(Name).ToPoint() + new Point(32, 32);
    private Point Position
    {
        get
        {
            var buildingPosition = new Point(building.tileX.Value * 64 - Game1.viewport.X, building.tileY.Value * 64 - Game1.viewport.Y);
            return new Point(buildingPosition.X - Size.X / 2 + Offset.X, buildingPosition.Y - Size.Y / 2 + Offset.Y);
        }
    }

    private Point Offset => new(config.XOffset, config.YOffset);

    public CabinOwnerNameBox(Building building, Cabin cabin, ModConfig config)
    {
        this.building = building;
        this.cabin = cabin;
        this.config = config;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        IClickableMenu.drawTextureBox(spriteBatch, Position.X, Position.Y, Size.X, Size.Y, Color.White);
        Utility.drawTextWithShadow(spriteBatch, Name, Font, new Vector2(Position.X + 16, Position.Y + 16), TextColor, 1f, 1f);
    }
}