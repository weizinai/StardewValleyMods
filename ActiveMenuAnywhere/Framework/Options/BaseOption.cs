using Microsoft.Xna.Framework;

namespace ActiveMenuAnywhere.Framework.Options;

public abstract class BaseOption
{
    protected BaseOption(string name, Rectangle sourceRect)
    {
        Name = name;
        SourceRect = sourceRect;
    }

    public Rectangle SourceRect { get; }
    public string Name { get; }

    public virtual void ReceiveLeftClick()
    {
    }
}