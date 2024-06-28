using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

internal abstract class CabinMenu : IClickableMenu
{
    private const int WindowWidth = 576;
    private const int WindowHeight = 576;

    protected CabinMenu()
        : base(Game1.uiViewport.Width / 2 - WindowWidth / 2, Game1.uiViewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
    }
}