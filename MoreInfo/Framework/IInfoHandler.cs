using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.MoreInfo.Framework;

internal interface IInfoHandler
{
    public Vector2 Position { get; set; }

    public void Init(IModEvents modEvents);

    public void Clear(IModEvents modEvents);

    public bool IsEnable();

    public void Draw(SpriteBatch b);

    public void UpdateHover(Vector2 mousePosition);

    public void DrawHoverText(SpriteBatch b);
}