// ReSharper disable MemberCanBePrivate.Global

namespace SomeMultiplayerFeature.Framework.Message;

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
        return IsExit ? $"{PlayerName}离开了{ShopId}" : $"{PlayerName}访问了{ShopId}";
    }
}