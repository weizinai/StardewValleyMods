using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class AccessShopInfoHandler : BaseHandlerWithConfig<ModConfig>
{
    public AccessShopInfoHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Init()
    {
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is ShopMenu shopMenu1)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu1.ShopId);
            this.Helper.Multiplayer.SendMessage(message, "AccessShopInfoMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
        else if (e.OldMenu is ShopMenu shopMenu2)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu2.ShopId, true);
            this.Helper.Multiplayer.SendMessage(message, "AccessShopInfoMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
    }

    // 当收到来自其他玩家的商店访问信息时，显示HUD信息
    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!this.Config.ShowAccessShopInfo) return;

        // 如果当前没有玩家在线，则返回
        if (!Context.HasRemotePlayers) return;

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "AccessShopInfoMessage" })
        {
            var message = e.ReadAs<AccessShopInfoMessage>();
            var hudMessage = new HUDMessage(message.ToString())
            {
                noIcon = true,
                timeLeft = 500f,
                type = message.PlayerName + message.IsExit
            };
            Game1.addHUDMessage(hudMessage);
        }
    }
}