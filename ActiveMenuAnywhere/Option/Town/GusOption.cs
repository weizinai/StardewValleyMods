using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class GusOption : BaseOption
{
    public GusOption(Rectangle sourceRect) :
        base(I18n.Option_Gus(), sourceRect) { }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}