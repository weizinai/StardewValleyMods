using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    public string Name { get; }
    public Rectangle SourceRect { get; }
    public float Scale { get; set; } = 1f;

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