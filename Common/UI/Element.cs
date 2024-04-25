using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public abstract class Element
{
    public abstract Vector2 LocalPosition { get; set; }
    public Vector2 Position => LocalPosition + (Parent?.Position ?? Vector2.Zero);
    public abstract int Width { get; }
    public abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
    
    public Container? Parent;

    public bool Hover;
    public Action<SpriteBatch>? OnHover;
    public Action? OffHover;

    public Func<bool>? CheckHidden;

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

    public virtual void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        if (Hover)
            OnHover?.Invoke(spriteBatch);
        else
            OffHover?.Invoke();
    }

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}