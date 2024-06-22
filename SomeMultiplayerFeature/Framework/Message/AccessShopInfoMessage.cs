// ReSharper disable MemberCanBePrivate.Global

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework.Message;

public class AccessShopInfoMessage
{
    public readonly string PlayerName;
    public readonly string ShopId;
    public readonly bool IsExit;

    public AccessShopInfoMessage(string playerName, string shopId, bool isExit = false)
    {
        PlayerName = playerName;
        ShopId = shopId;
        IsExit = isExit;
    }

    public override string ToString()
    {
        return IsExit ? I18n.UI_ExitShop(PlayerName, ShopId) : I18n.UI_AccessShop(PlayerName, ShopId);
    }
}