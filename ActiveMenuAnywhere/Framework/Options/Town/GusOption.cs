using Microsoft.Xna.Framework;
using StardewValley;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework.Options;

internal class GusOption : BaseOption
{
    public GusOption(Rectangle sourceRect) :
        base(I18n.Option_Gus(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("Saloon", "Gus");
    }
}