using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MoreInfo.Framework;

internal abstract class BaseInfoHandler : IInfoHandler
{
    private bool isHover;
    
    protected Texture2D Texture = null!;
    protected Rectangle SourceRectangle;
    protected Rectangle Bound => new((int)this.Position.X, (int)this.Position.Y, 64, 64);
    protected abstract string HoverText { get; }
    
    public Vector2 Position { get; set; }

    public abstract void Init(IModEvents modEvents);

    public abstract void Clear(IModEvents modEvents);

    public abstract bool IsEnable();

    public abstract void Draw(SpriteBatch b);

    public virtual void UpdateHover(Vector2 mousePosition)
    {
        this.isHover = this.Bound.Contains(mousePosition);
    }

    public virtual void DrawHoverText(SpriteBatch b)
    {
        if (this.isHover) IClickableMenu.drawHoverText(b, this.HoverText, Game1.smallFont);
    }

    protected string GetStringFromDictionary(Dictionary<string, int> info)
    {
        var stringBuilder = new StringBuilder();
        foreach (var (key, value) in info)
            stringBuilder.AppendLine($"{key}: {value}");
        stringBuilder.Length--;

        return stringBuilder.ToString();
    }
}