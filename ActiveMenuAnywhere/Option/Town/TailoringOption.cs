using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class TailoringOption : BaseOption
{
    public TailoringOption(Rectangle sourceRect) :
        base(I18n.UI_Option_Tailoring(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("992559");
    }

    public override void Apply()
    {
        Game1.activeClickableMenu = new TailoringMenu();
    }
}