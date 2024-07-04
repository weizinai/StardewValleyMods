using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    public string Name { get; }
    public Rectangle SourceRect { get; }

    protected BaseOption(string name, Rectangle sourceRect)
    {
        this.Name = name;
        this.SourceRect = sourceRect;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();
}