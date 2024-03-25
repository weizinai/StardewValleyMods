using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.ActiveMenu;

public class TVMenu : BaseActiveMenu
{
    private readonly IModHelper helper;

    public TVMenu(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}