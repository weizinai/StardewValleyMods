using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.MoreInfo.Framework;

internal abstract class BaseInfoHandler : IInfoHandler
{
    private bool isHover;

    protected Texture2D Texture = null!;
    protected Rectangle SourceRectangle;
    protected abstract Rectangle Bound { get; }
    protected abstract string HoverText { get; }

    public Vector2 Position { get; set; }

    public virtual void Init(IModEvents modEvents) { }

    public virtual void Clear(IModEvents modEvents) { }

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
}