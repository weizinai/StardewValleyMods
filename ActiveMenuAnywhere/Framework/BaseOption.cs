using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

internal abstract class BaseOption
{
    public OptionId Id { get; }

    internal string Label { get; }

    internal Texture2D Texture { get; }

    internal Rectangle SourceRect { get; }

    protected BaseOption(string label, Texture2D texture, Rectangle sourceRect, OptionId id)
    {
        this.Label = label;
        this.Texture = texture;
        this.SourceRect = sourceRect;
        this.Id = id;
    }

    public virtual bool IsEnable()
    {
        return true;
    }

    public abstract void Apply();

    protected static Rectangle GetSourceRectangle(int index)
    {
        var i = index % 3;
        var j = index / 3;

        return new Rectangle(i * 200, j * 200, 200, 200);
    }
}
