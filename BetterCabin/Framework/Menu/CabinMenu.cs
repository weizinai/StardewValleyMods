using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

internal abstract class CabinMenu : IClickableMenu
{
    private const int WindowWidth = 576;
    private const int WindowHeight = 576;
    
    protected readonly GameLocation OriginLocation;
    protected readonly Location OriginViewport;

    protected CabinMenu()
        : base(Game1.uiViewport.Width / 2 - WindowWidth / 2, Game1.uiViewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
        this.OriginLocation = Game1.player.currentLocation;
        this.OriginViewport = Game1.viewport.Location;
    }
}