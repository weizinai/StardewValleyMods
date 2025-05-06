using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class SpectatorMenu : IClickableMenu
{
    private ModConfig Config => ModConfig.Instance;

    private bool followPlayer;
    private bool randomSpectate;

    public bool RandomSpectate
    {
        get => this.randomSpectate;
        set
        {
            this.intervalTimer = 0;
            this.randomSpectate = value;
            Logger.NoIconHUDMessage(value ? I18n.UI_RandomSpectate_Begin() : I18n.UI_RandomSpectate_End());
        }
    }

    private int intervalTimer;

    private Farmer targetFarmer;
    private GameLocation targetLocation;

    private readonly GameLocation originLocation;
    private readonly Location originViewport;
    private readonly Point originTilePoint;
    private readonly int originHealth;

    public SpectatorMenu(GameLocation targetLocation, Farmer? targetFarmer = null)
    {
        // 初始化
        this.targetFarmer = targetFarmer ?? Game1.player;
        this.targetLocation = targetLocation;
        this.followPlayer = this.targetFarmer != Game1.player;

        this.originLocation = Game1.currentLocation;
        this.originViewport = Game1.viewport.Location;
        this.originTilePoint = Game1.player.TilePoint;
        this.originHealth = Game1.player.health;

        // 开始旁观
        this.BeginSpectate();
    }

    public override void update(GameTime time)
    {
        Game1.player.health = this.originHealth;

        if (this.RandomSpectate)
        {
            this.intervalTimer++;
            if (this.intervalTimer > this.Config.RandomSpectateInterval * 60)
            {
                if (this.targetFarmer != Game1.player)
                {
                    this.targetFarmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToArray());
                    this.targetLocation = this.targetFarmer.currentLocation;
                }
                else
                {
                    var newLocation = Game1.random.ChooseFrom(Game1.locations
                        .Where(location => Game1.player.locationsVisited.Contains(location.NameOrUniqueName))
                        .ToList()
                    );
                    this.targetLocation = newLocation;
                }
                this.BeginSpectate();
                this.intervalTimer = 0;
            }
        }

        if (this.followPlayer)
        {
            if (!this.targetLocation.Equals(this.targetFarmer.currentLocation))
            {
                this.targetLocation = this.targetFarmer.currentLocation;
                this.BeginSpectate();
            }

            Game1.viewport.Location = this.GetViewportFromFarmer();
            Game1.panScreen(0, 0);
            return;
        }

        PanScreenHelper.PanScreen(this.Config.MoveSpeed, this.Config.MoveThreshold);
    }

    public override void draw(SpriteBatch b)
    {
        if (ModConfig.Instance.ShowSpectateTooltip)
        {
            var title = this.followPlayer
                ? I18n.UI_SpectatorMode_Title(this.targetFarmer.displayName)
                : I18n.UI_SpectatorMode_Title(this.targetLocation.DisplayName);
            SpriteText.drawStringWithScrollCenteredAt(b, title, Game1.uiViewport.Width / 2, 64);
        }

        if (this.Config.ShowRandomSpectateTooltip && this.RandomSpectate)
        {
            SpriteText.drawStringWithScrollCenteredAt(
                b,
                I18n.UI_RandomSpectate_Title(this.intervalTimer / 60, this.Config.RandomSpectateInterval),
                Game1.uiViewport.Width / 2,
                144
            );
        }

        if (this.Config.ShowTimeAndMoney) Game1.dayTimeMoneyBox.draw(b);

        if (this.Config.ShowToolbar) this.DrawToolBar(b);

        this.drawMouse(b);
    }

    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);

        if (this.Config.ToggleStateKey.JustPressed()) this.followPlayer = !this.followPlayer;

        if (this.Config.RandomSpectateKey.JustPressed()) this.RandomSpectate = !this.RandomSpectate;
    }

    protected override void cleanupBeforeExit()
    {
        var locationRequest = Game1.getLocationRequest(this.originLocation.NameOrUniqueName);
        locationRequest.OnWarp += () =>
        {
            Game1.displayFarmer = true;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = this.originViewport;
        };
        Game1.warpFarmer(locationRequest, this.originTilePoint.X, this.originTilePoint.Y, Game1.player.FacingDirection);
    }

    private void BeginSpectate()
    {
        var locationRequest = Game1.getLocationRequest(this.targetLocation.NameOrUniqueName);
        locationRequest.OnWarp += () =>
        {
            Game1.displayFarmer = false;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = this.GetInitialViewport();
        };
        Game1.warpFarmer(locationRequest, 0, 0, Game1.player.FacingDirection);
    }

    private Location GetInitialViewport()
    {
        if (this.followPlayer) return this.GetViewportFromFarmer();

        var layer = this.targetLocation.Map.Layers[0];

        return new Location(
            layer.LayerWidth / 2 * 64 - Game1.viewport.Width / 2,
            layer.LayerHeight / 2 * 64 - Game1.viewport.Height / 2
        );
    }

    private Location GetViewportFromFarmer()
    {
        var x = (int)this.targetFarmer.Position.X - Game1.viewport.Width / 2;
        var y = (int)this.targetFarmer.Position.Y - Game1.viewport.Height / 2;
        return new Location(x, y);
    }

    private void DrawToolBar(SpriteBatch b)
    {
        var bound = new Rectangle(Game1.uiViewport.Width / 2 - 400, Game1.uiViewport.Height - 96 - 8, 800, 96);

        drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
            bound.X, bound.Y, bound.Width, bound.Height, Color.White, 1f, false);

        for (var i = 0; i < 12; i++)
        {
            b.Draw(Game1.menuTexture, new Vector2(bound.X + 16 + i * 64, bound.Y + 16),
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, this.targetFarmer.CurrentToolIndex == i ? 56 : 10), Color.White);
        }

        for (var i = 0; i < 12; i++)
        {
            if (this.targetFarmer.Items.Count > i && this.targetFarmer.Items[i] != null)
            {
                this.targetFarmer.Items[i].drawInMenu(b, new Vector2(bound.X + 16 + i * 64, bound.Y + 16),
                    this.targetFarmer.CurrentToolIndex == i ? 0.9f : 0.8f);
            }
        }
    }
}