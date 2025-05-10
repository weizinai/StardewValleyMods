using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class TailoringOption : BaseOption
{
    public TailoringOption()
        : base(I18n.UI_Option_Tailoring(), TextureManager.Instance.TownTexture, GetSourceRectangle(12), OptionId.Tailoring) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("992559");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new TailoringMenu();
    }
}