using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    protected bool Hover;
    public Action<SpriteBatch>? OnHover;
    public Action? OffHover;

    private bool leftClickGesture;
    private bool LeftClick => Hover && leftClickGesture;
    public Action? OnLeftClick;

    public Func<bool>? CheckHidden;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            Hover = false;
            leftClickGesture = false;
            return;
        }
        
        var mousePosition = Game1.getMousePosition();
        Hover = Bounds.Contains(mousePosition);
        leftClickGesture = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed;
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

    public virtual void ReceiveLeftClick()
    {
        if (IsHidden()) return;
        if (LeftClick) OnLeftClick?.Invoke();
    }

    public bool IsHidden()
    {
        return CheckHidden is not null && CheckHidden.Invoke();
    }
}