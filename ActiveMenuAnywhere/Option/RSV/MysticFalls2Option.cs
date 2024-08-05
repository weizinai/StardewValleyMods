using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MysticFalls2Option : BaseOption
{
    public MysticFalls2Option() : base(I18n.UI_Option_MysticFall2(), GetSourceRectangle(13)) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160187");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVMysticFalls2", null, false);
    }
}