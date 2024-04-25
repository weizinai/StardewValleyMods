using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public abstract class Container : Element
{
    protected bool UpdateChildren = true;
    public readonly List<Element> Children = new();

    public void AddChild(params Element[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent?.RemoveChild(element);
            Children.Add(element);
            element.Parent = this;
        }
    }

    private void RemoveChild(Element element)
    {
        if (element.Parent != this) throw new ArgumentException("Element must be a child of this container.");
        Children.Remove(element);
        element.Parent = null;
    }

    public override void Update()
    {
        base.Update();
        if (UpdateChildren) foreach (var element in Children) element.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.Draw(spriteBatch);
    }
    
    public override void PerformHoverAction(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.PerformHoverAction(spriteBatch);
    }
}