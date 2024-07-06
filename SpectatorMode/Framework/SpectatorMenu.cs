using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace weizinai.StardewValleyMod.SpectatorMode.Framework;

internal class SpectatorMenu : IClickableMenu
{
    private readonly ModConfig config = ModEntry.Config;

    private bool followPlayer;
    private bool randomSpectate;
    private bool RandomSpectate
    {
        get => this.randomSpectate;
        set
        {
            this.intervalTimer = 0;
            this.randomSpectate = value;
            Log.NoIconHUDMessage(value ? I18n.UI_RandomSpectate_Begin() : I18n.UI_RandomSpectate_End());
        } 
    }

    private int intervalTimer;

    private Farmer targetFarmer;
    private GameLocation targetLocation;

    private readonly GameLocation originLocation;
    private readonly Location originViewport;

    public SpectatorMenu(GameLocation targetLocation, Farmer? targetFarmer = null)
    {
        // 初始化
        this.targetFarmer = targetFarmer ?? Game1.player;
        this.targetLocation = targetLocation;
        this.followPlayer = this.targetFarmer != Game1.player;

        this.originLocation = Game1.currentLocation;
        this.originViewport = Game1.viewport.Location;

        // 切换视角
        Game1.globalFadeToBlack(this.Init);
    }

    public override void update(GameTime time)
    {
        if (this.RandomSpectate)
        {
            this.intervalTimer++;
            if (this.intervalTimer > this.config.RandomSpectateInterval * 60)
            {
                if (this.targetFarmer != Game1.player)
                {
                    this.targetFarmer = Game1.random.ChooseFrom(Game1.otherFarmers.Values.ToArray());
                    this.InitLocationData(this.targetLocation, this.targetFarmer.currentLocation);
                    this.targetLocation = this.targetFarmer.currentLocation;
                }
                else
                {
                    var newLocation = Game1.random.ChooseFrom(Game1.locations.Where(location => Game1.player.locationsVisited.Contains(location.NameOrUniqueName)).ToList());
                    this.InitLocationData(this.targetLocation, newLocation);
                    this.targetLocation = newLocation;
                    Game1.viewport.Location = this.GetInitialViewport();
                }
                this.intervalTimer = 0;
            }
        }
        
        if (this.followPlayer)
        {
            if (!this.targetLocation.Equals(this.targetFarmer.currentLocation))
            {
                this.InitLocationData(this.targetLocation, this.targetFarmer.currentLocation);
                this.targetLocation = this.targetFarmer.currentLocation;
            }

            Game1.viewport.Location = this.GetViewportFromFarmer();
            Game1.panScreen(0, 0);
            return;
        }

        PanScreenHelper.PanScreen(this.config.MoveSpeed, this.config.MoveThreshold);
    }

    public override void draw(SpriteBatch b)
    {
        var title = this.followPlayer
            ? I18n.UI_SpectatorMode_Title(this.targetFarmer.displayName)
            : I18n.UI_SpectatorMode_Title(this.targetLocation.DisplayName);
        SpriteText.drawStringWithScrollCenteredAt(b, title, Game1.uiViewport.Width / 2, 64);

        if (this.RandomSpectate)
        {
            SpriteText.drawStringWithScrollCenteredAt(b, I18n.UI_RandomSpectate_Title(this.intervalTimer / 60, this.config.RandomSpectateInterval),
                Game1.uiViewport.Width / 2, 144);
        }

        if (this.config.ShowTimeAndMoney) Game1.dayTimeMoneyBox.draw(b);

        if (this.config.ShowToolbar) this.DrawToolBar(b);

        this.drawMouse(b);
    }

    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);

        if (this.config.ToggleStateKey.JustPressed()) this.followPlayer = !this.followPlayer;

        if (this.config.RandomSpectateKey.JustPressed()) this.RandomSpectate = !this.RandomSpectate;
    }

    protected override void cleanupBeforeExit()
    {
        var locationRequest = Game1.getLocationRequest(this.originLocation.NameOrUniqueName);
        locationRequest.OnWarp += delegate
        {
            Game1.player.viewingLocation.Value = null;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = this.originViewport;
            Game1.displayFarmer = true;
            Game1.displayHUD = true;
        };
        Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
    }

    private void Init()
    {
        this.InitLocationData(Game1.currentLocation, this.targetLocation);
        Game1.displayFarmer = false;
        Game1.displayHUD = false;
        Game1.viewportFreeze = true;
        Game1.viewport.Location = this.GetInitialViewport();
        Game1.globalFadeToClear();
    }

    private void InitLocationData(GameLocation oldLocation, GameLocation newLocation)
    {
        oldLocation.cleanupBeforePlayerExit();
        Game1.currentLocation = newLocation;
        Game1.player.viewingLocation.Value = newLocation.NameOrUniqueName;
        newLocation.resetForPlayerEntry();
    }

    private Location GetInitialViewport()
    {
        if (this.followPlayer) return this.GetViewportFromFarmer();

        var layer = this.targetLocation.Map.Layers[0];
        return new Location(layer.LayerWidth / 2 * 64 - Game1.viewport.Width / 2, layer.LayerHeight / 2 * 64 - Game1.viewport.Height / 2);
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