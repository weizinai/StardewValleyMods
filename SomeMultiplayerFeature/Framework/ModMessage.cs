// ReSharper disable MemberCanBePrivate.Global

namespace SomeMultiplayerFeature.Framework;

public class ModMessage
{
    public readonly string PlayerName;
    public readonly string ShopId;
    public readonly bool IsExit;

    public ModMessage(string playerName, string shopId, bool isExit = false)
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