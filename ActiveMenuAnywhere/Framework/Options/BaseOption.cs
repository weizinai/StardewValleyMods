using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.Options;

public abstract class BaseOption : ClickableTextureComponent
{
    protected BaseOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, string name) : base(bounds, texture, sourceRect, 1f)
    {
        this.name = name;
    }

    public virtual void ReceiveLeftClick()
    {
    }

    public override void draw(SpriteBatch spriteBatch)
    {
        base.draw(spriteBatch);
        var x = bounds.X + bounds.Width / 2;
        var y = bounds.Y + bounds.Height / 3 * 2;
        var width = Game1.smallFont.MeasureString(name).X;
        DrawHelper.DrawTab(x,y,Game1.smallFont,name,Align.Center);
    }
}