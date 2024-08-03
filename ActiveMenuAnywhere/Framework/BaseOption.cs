using Microsoft.Xna.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    public string Label { get; }
    public Rectangle SourceRect { get; }
    public float Scale { get; set; } = 1f;

    protected BaseOption(string label, Rectangle sourceRect)
    {
        this.Label = label;
        this.SourceRect = sourceRect;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();
}