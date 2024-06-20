using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace ObserverMode.Framework;

internal class ObserverMenu : IClickableMenu
{
    private readonly GameLocation targetLocation;

    public ObserverMenu(GameLocation targetLocation)
    {
        this.targetLocation = targetLocation;
        Init();
    }

    public override void update(GameTime time)
    {
        int mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
        int mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
        if (mouseX - Game1.viewport.X < 64)
        {
            Game1.panScreen(-32, 0);
        }
        else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -128)
        {
            Game1.panScreen(32, 0);
        }
        if (mouseY - Game1.viewport.Y < 64)
        {
            Game1.panScreen(0, -32);
        }
        else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
        {
            Game1.panScreen(0, 32);
        }
    }

    private void Init()
    {
        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.currentLocation = targetLocation;
        Game1.player.viewingLocation.Value = targetLocation.NameOrUniqueName;
        Game1.currentLocation.resetForPlayerEntry();
        Game1.globalFadeToClear();
        Game1.displayHUD = false;
        Game1.viewportFreeze = true;
        Game1.viewport.Location = new Location(0, 0);
        Game1.clampViewportToGameMap();
        Game1.panScreen(0, 0);
        Game1.displayFarmer = true;
    }
}