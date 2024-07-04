using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class BobberOption : BaseOption
{
    public BobberOption(Rectangle sourceRect) :
        base(I18n.Option_Bobber(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.mailReceived.Contains("spring_2_1");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new ChooseFromIconsMenu("bobbers");
    }
}