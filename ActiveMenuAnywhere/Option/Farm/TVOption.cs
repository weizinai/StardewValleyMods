using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(IModHelper helper)
    {
        this.helper = helper;
    }

    public override void Apply()
    {
        this.helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}
