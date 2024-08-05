using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(IModHelper helper) :
        base(I18n.UI_Option_TV(), GetSourceRectangle(0))
    {
        this.helper = helper;
    }

    public override void Apply()
    {
        this.helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}