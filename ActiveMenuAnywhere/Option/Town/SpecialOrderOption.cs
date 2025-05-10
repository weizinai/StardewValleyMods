using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class SpecialOrderOption : BaseOption
{
    public SpecialOrderOption()
        : base(I18n.UI_Option_SpecialOrder(), TextureManager.Instance.TownTexture, GetSourceRectangle(1), OptionId.SpecialOrder) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("15389722");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard();
    }
}