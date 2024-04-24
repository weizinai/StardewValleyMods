using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.UI;

public abstract class Container : Element
{
    public List<Element> Children = new();

    public Container(params Element[] children)
    {
        foreach (var element in children) AddChild(element);
    }

    public void AddChild(Element element)
    {
        element.Parent?.RemoveChild(element);
        Children.Add(element);
        element.Parent = this;
    }
    
    public void RemoveChild(Element element)
    {
        if (element.Parent != this) throw new ArgumentException("Element must be a child of this container.");
        Children.Remove(element);
        element.Parent = null;
    }
    
    public override void Update()
    {
        base.Update();
        foreach (var element in Children) element.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsHidden()) return;
        foreach (var element in Children) element.Draw(spriteBatch);
    }
}