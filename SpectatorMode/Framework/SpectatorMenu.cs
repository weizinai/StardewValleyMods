using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace SpectatorMode.Framework;

internal class SpectatorMenu : IClickableMenu
{
    private readonly ModConfig config;


    private bool followPlayer;
    private bool FollowPlayer
    {
        get => followPlayer;
        set
        {
            followPlayer = value;
            title = value ? new MenuTitle(I18n.UI_SpectatorMode_Title(targetFarmer.displayName)) : new MenuTitle(I18n.UI_SpectatorMode_Title(targetLocation.DisplayName));
        }
    }
    private readonly Farmer targetFarmer;
    private readonly GameLocation targetLocation;
    private MenuTitle title = null!;

    private static GameLocation originLocation = null!;
    private static Location originViewport;

    public SpectatorMenu(ModConfig config, GameLocation targetLocation, Farmer? targetFarmer = null, bool followPlayer = false, bool firstActive = true)
    {
        // 初始化
        this.config = config;
        this.targetFarmer = targetFarmer ?? Game1.player;
        this.targetLocation = targetLocation;
        FollowPlayer = followPlayer;

        // 储存原始数据
        if (firstActive)
        {
            originLocation = Game1.player.currentLocation;
            originViewport = Game1.viewport.Location;
        }

        // 切换视角
        Game1.globalFadeToBlack(Init);
    }

    public override void update(GameTime time)
    {
        if (FollowPlayer)
        {
            if (!targetLocation.Equals(targetFarmer.currentLocation))
                Game1.activeClickableMenu = new SpectatorMenu(config, targetFarmer.currentLocation, targetFarmer, true, false);

            Game1.viewport.Location = GetViewportFromFarmer();
            Game1.clampViewportToGameMap();
        }

        // 鼠标移动视角
        var mouseX = Game1.getOldMouseX(false);
        var mouseY = Game1.getOldMouseY(false);
        var moveSpeed = config.MoveSpeed;
        var moveThreshold = config.MoveThreshold;

        // 水平移动
        if (mouseX < moveThreshold)
        {
            FollowPlayer = false;
            Game1.panScreen(-moveSpeed, 0);
        }
        else if (mouseX - Game1.viewport.Width >= -moveThreshold)
        {
            FollowPlayer = false;
            Game1.panScreen(moveSpeed, 0);
        }

        // 垂直移动
        if (mouseY < moveThreshold)
        {
            FollowPlayer = false;
            Game1.panScreen(0, -moveSpeed);
        }
        else if (mouseY - Game1.viewport.Height >= -moveThreshold)
        {
            FollowPlayer = false;
            Game1.panScreen(0, moveSpeed);
        }
    }

    public override void draw(SpriteBatch b)
    {
        title.Draw(b);
        
        drawMouse(b);
    }

    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);

        var moveSpeed = config.MoveSpeed;

        if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
        {
            FollowPlayer = false;
            Game1.panScreen(0, moveSpeed);
        }
        else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        {
            FollowPlayer = false;
            Game1.panScreen(moveSpeed, 0);
        }
        else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
        {
            FollowPlayer = false;
            Game1.panScreen(0, -moveSpeed);
        }
        else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        {
            FollowPlayer = false;
            Game1.panScreen(-moveSpeed, 0);
        }
    }

    protected override void cleanupBeforeExit()
    {
        var locationRequest = Game1.getLocationRequest(originLocation.NameOrUniqueName);
        locationRequest.OnWarp += delegate
        {
            Game1.player.viewingLocation.Value = null;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = originViewport;
            Game1.displayFarmer = true;
        };
        Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
    }

    private void Init()
    {
        InitLocationData(Game1.currentLocation, targetLocation);
        Game1.globalFadeToClear();
        Game1.viewportFreeze = true;
        Game1.viewport.Location = GetInitialViewport();
        Game1.displayFarmer = false;
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
        if (FollowPlayer)
        {
            return GetViewportFromFarmer();
        }

        var layer = targetLocation.Map.Layers[0];
        return new Location(layer.LayerWidth / 2, layer.LayerHeight / 2);
    }

    private Location GetViewportFromFarmer()
    {
        var x = (int)targetFarmer.Position.X - Game1.viewport.Width / 2;
        var y = (int)targetFarmer.Position.Y - Game1.viewport.Height / 2;
        return new Location(x, y);
    }
}