using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public abstract class BaseActiveMenu: ClickableTextureComponent
{
    public BaseActiveMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect):base(bounds, texture, sourceRect, 1f)
    {
        
    }

    public virtual void ReceiveLeftClick()
    {
        
    }
}