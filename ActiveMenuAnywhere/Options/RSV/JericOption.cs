using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Options;

internal class JericOption : BaseOption
{
    public JericOption(Rectangle sourceRect) :
        base(I18n.Option_Jeric(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        Utility.TryOpenShopMenu("RSVJericShop", "Jeric");
    }
}