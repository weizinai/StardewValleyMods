using Microsoft.Xna.Framework;
using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class HatMouseOption : BaseOption
{
    public HatMouseOption(Rectangle sourceRect) :
        base(I18n.Option_HatMouse(), sourceRect) { }

    public override bool IsEnable()
    {
        return Game1.player.achievements.Count > 0;
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("HatMouse", null, true);
    }
}