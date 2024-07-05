using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace MoreInfo.Framework;

internal abstract class BaseInfoHandler : IInfoHandler
{
    protected bool IsHover;
    
    public abstract void Init(IModEvents modEvents);
    
    public abstract void Clear(IModEvents modEvents);
    
    public abstract bool IsEnable();
    
    public abstract void Draw(SpriteBatch b);

    public abstract void UpdateHover(Vector2 mousePosition);

    public abstract void DrawHoverText(SpriteBatch b);
}