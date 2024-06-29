using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(Rectangle sourceRect, IModHelper helper) :
        base(I18n.Option_TV(), sourceRect)
    {
        this.helper = helper;
    }

    public override void ReceiveLeftClick()
    {
        this.helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}