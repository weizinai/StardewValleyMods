using StardewValley;

namespace Common.UI;

public class RootElement : Container
{
    protected override int Width => Game1.viewport.Width;
    protected override int Height => Game1.viewport.Height;
}