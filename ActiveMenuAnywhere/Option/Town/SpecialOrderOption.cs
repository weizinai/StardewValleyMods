using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class SpecialOrderOption : BaseOption
{
    public SpecialOrderOption(Rectangle sourceRect) :
        base(I18n.Option_SpecialOrder(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.MasterPlayer.eventsSeen.Contains("15389722");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new SpecialOrdersBoard();
    }
}