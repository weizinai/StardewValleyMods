using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ActiveMenuAnywhere.Framework.Options;

public class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_TV(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}