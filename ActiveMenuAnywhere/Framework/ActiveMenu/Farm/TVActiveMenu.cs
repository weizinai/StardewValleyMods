using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.ActiveMenu.Farm;

public class TVActiveMenu : BaseActiveMenu
{
    private readonly IModHelper helper;
    
    public TVActiveMenu(IModHelper helper, Rectangle bounds, Texture2D texture, Rectangle sourceRect):base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}