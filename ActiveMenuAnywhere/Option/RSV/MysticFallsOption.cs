using StardewValley;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Catalog;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

/// <summary>
/// 神秘瀑布商店选项：RSV 神秘瀑布的三个商店（1/2/3）行为一致，仅店铺 id 不同，
/// 由构造参数承载，目录行分别以对应店铺 id 物化。
/// </summary>
internal class MysticFallsOption : BaseOption
{
    private readonly string shopId;

    public MysticFallsOption(string shopId)
    {
        this.shopId = shopId;
    }

    public override bool IsEnable()
    {
        return Game1.player.eventsSeen.Contains("75160187");
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu(this.shopId, null, false);
    }
}
