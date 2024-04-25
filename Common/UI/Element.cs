using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Common.UI;

public abstract class Element
{
    public abstract Vector2 LocalPosition { get; set; }
    public Vector2 Position => LocalPosition + (Parent?.Position ?? Vector2.Zero);
    protected abstract int Width { get; }
    protected abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
    
    public Container? Parent;

    private bool hover;
    public Action<SpriteBatch>? OnHover;
    public Action? OffHover;

    public Func<bool>? CheckHidden;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            hover = false;
            return;
        }
        
        var mousePosition = Game1.getMousePosition();
        hover = Bounds.Contains(mousePosition);
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        if (hover)
            OnHover?.Invoke(spriteBatch);
        else
            OffHover?.Invoke();
    }

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}