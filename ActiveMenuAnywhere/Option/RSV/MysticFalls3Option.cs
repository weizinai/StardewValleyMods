using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

public class MysticFalls3Option : BaseOption
{
    public MysticFalls3Option()
        : base(I18n.UI_Option_MysticFall3(), TextureManager.Instance.RSVTexture, GetSourceRectangle(14), OptionId.MysticFalls3) { }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160187");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("RSVMysticFalls3", null, false);
    }
}