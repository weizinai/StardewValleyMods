using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    protected BaseOption(string name, Rectangle sourceRect)
    {
        this.Name = name;
        this.SourceRect = sourceRect;
    }

    public Rectangle SourceRect { get; }
    public string Name { get; }

    public virtual void ReceiveLeftClick()
    {
    }
}