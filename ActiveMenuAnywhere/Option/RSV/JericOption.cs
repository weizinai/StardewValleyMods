using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class JericOption : BaseOption
{
    public JericOption(Rectangle sourceRect) :
        base(I18n.Option_Jeric(), sourceRect) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}