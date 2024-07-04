using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class KrobusOption : BaseOption
{
    public KrobusOption(Rectangle sourceRect) :
        base(I18n.Option_Krobus(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.hasRustyKey;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("ShadowShop", "Krobus");
    }
}