using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class TVOption : BaseOption
{
    private readonly IModHelper helper;

    public TVOption(IModHelper helper)
        : base(I18n.UI_Option_TV(), TextureManager.Instance.FarmTexture, GetSourceRectangle(0), OptionId.TV)
    {
        this.helper = helper;
    }

    public override void Apply()
    {
        this.helper.Reflection.GetMethod(new TV(), "checkForAction").Invoke(Game1.player, false);
    }
}