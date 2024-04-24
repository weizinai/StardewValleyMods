using Microsoft.Xna.Framework;
using StardewValley;

namespace Common.UI;

public class RootElement : Container
{
    public override Vector2 LocalPosition { get; set; } = Vector2.Zero;
    public override int Width => Game1.viewport.Width;
    public override int Height => Game1.viewport.Height;
}