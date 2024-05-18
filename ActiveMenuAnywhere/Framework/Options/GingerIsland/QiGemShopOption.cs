using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace ActiveMenuAnywhere.Framework.Options;

internal class QiGemShopOption : BaseOption
{
    public QiGemShopOption(Rectangle sourceRect) :
        base(I18n.Option_QiGemShop(), sourceRect)
    {
    }

    public override void ReceiveLeftClick()
    {
        var isQiWalnutRoomDoorUnlocked = IslandWest.IsQiWalnutRoomDoorUnlocked(out _);
        if (isQiWalnutRoomDoorUnlocked)
            Utility.TryOpenShopMenu("QiGemShop", null, true);
        else
            Game1.drawObjectDialogue(I18n.Tip_Unavailable());
    }
}