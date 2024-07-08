// ReSharper disable MemberCanBePrivate.Global

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

public class AccessShopInfoMessage
{
    public readonly string PlayerName;
    public readonly string ShopId;
    public readonly bool IsExit;

    public AccessShopInfoMessage(string playerName, string shopId, bool isExit = false)
    {
        this.PlayerName = playerName;
        this.ShopId = shopId;
        this.IsExit = isExit;
    }

    public override string ToString()
    {
        return this.IsExit ? I18n.UI_ExitShop(this.PlayerName, this.ShopId) : I18n.UI_AccessShop(this.PlayerName, this.ShopId);
    }
}