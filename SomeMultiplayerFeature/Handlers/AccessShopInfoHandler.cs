using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Framework.Message;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Handlers;

/// <summary>
/// 显示玩家是否正在访问商店
/// </summary>
internal class AccessShopInfoHandler
{
    private readonly IModHelper helper;
    private readonly ModConfig config;
    private IClickableMenu? lastShopMenu;

    public AccessShopInfoHandler(IModHelper helper, ModConfig config)
    {
        this.helper = helper;
        this.config = config;
    }

    public void Update()
    {
        if (Game1.activeClickableMenu is ShopMenu shopMenu1 && lastShopMenu is not ShopMenu)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu1.ShopId);
            helper.Multiplayer.SendMessage(message, "ShopMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }
        else if (lastShopMenu is ShopMenu shopMenu2 && Game1.activeClickableMenu is not ShopMenu)
        {
            var message = new AccessShopInfoMessage(Game1.player.Name, shopMenu2.ShopId, true);
            helper.Multiplayer.SendMessage(message, "ShopMessage", new[] { "weizinai.SomeMultiplayerFeature" });
        }

        lastShopMenu = Game1.activeClickableMenu;
    }
    
    public void OnModMessageReceived(ModMessageReceivedEventArgs e)
    {
        if (config.AccessShopInfo && e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "ShopMessage" })
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