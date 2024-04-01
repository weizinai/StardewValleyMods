using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.Options;

public class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(Rectangle bounds, Texture2D texture, Rectangle sourceRect, IModHelper helper) : base(bounds, texture, sourceRect,I18n.Option_TV())
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}