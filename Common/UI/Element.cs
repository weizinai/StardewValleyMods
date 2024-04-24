using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public abstract class Element
{
    public abstract Vector2 Position { get; set; }
    public abstract int Width { get; }
    public abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

    public bool Hover;

    public Func<bool>? CheckHidden = null;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            Hover = false;
            return;
        }
        
        var mousePosition = Game1.getMousePosition();
        Hover = Bounds.Contains(mousePosition);
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}