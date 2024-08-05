using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class MysticFalls1Option : BaseOption
{
    public MysticFalls1Option()
        : base(I18n.UI_Option_MysticFall1(), TextureManager.Instance.RSVTexture,GetSourceRectangle(12), OptionId.MysticFalls1) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160187");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVMysticFalls1", null, false);
    }
}