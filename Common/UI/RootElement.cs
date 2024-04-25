using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public class RootElement : Container
{
    public override Vector2 LocalPosition { get; set; } = Vector2.Zero;
    protected override int Width => Game1.viewport.Width;
    protected override int Height => Game1.viewport.Height;
}