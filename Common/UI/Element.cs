using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace Common.UI;

public abstract class Element
{
    public Vector2 LocalPosition { get; set; }
    protected Vector2 Position => LocalPosition + (Parent?.Position ?? Vector2.Zero);
    protected abstract int Width { get; }
    protected abstract int Height { get; }
    private Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
    
    public Container? Parent;

    private bool hover;
    public Action<Element>? OnHover;
    public Action<Element>? OffHover;

    private bool leftClickGesture;
    private bool LeftClick => hover && leftClickGesture;
    public Action? OnLeftClick;

    public Func<bool>? CheckHidden;

    public virtual void Update()
    {
        var isHidden = IsHidden();
        if (isHidden)
        {
            hover = false;
            leftClickGesture = false;
            return;
        }
        
        var mousePosition = Game1.getMousePosition();
        hover = Bounds.Contains(mousePosition);
        leftClickGesture = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Pressed;
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    public virtual void PerformHoverAction()
    {
        if (IsHidden()) return;
        if (hover)
            OnHover?.Invoke(this);
        else
            OffHover?.Invoke(this);
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