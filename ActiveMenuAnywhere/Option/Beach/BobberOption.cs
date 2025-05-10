using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class BobberOption : BaseOption
{
    public BobberOption()
        : base(I18n.UI_Option_Bobber(), TextureManager.Instance.BeachTexture, GetSourceRectangle(1), OptionId.Bobber) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("spring_2_1");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new ChooseFromIconsMenu("bobbers");
    }
}