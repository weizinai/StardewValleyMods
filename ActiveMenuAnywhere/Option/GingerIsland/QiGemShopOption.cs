using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.ActiveMenuAnywhere.Framework;

namespace weizinai.StardewValleyMod.ActiveMenuAnywhere.Option;

internal class QiGemShopOption : BaseOption
{
    public QiGemShopOption(Rectangle sourceRect) :
        base(I18n.Option_QiGemShop(), sourceRect) { }
    
    public override bool IsEnable()
    {
        return IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
    }

    public override void Apply()
    {
        Utility.TryOpenShopMenu("QiGemShop", null, true);
    }
}