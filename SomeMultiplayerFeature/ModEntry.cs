using Common.Patch;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new UtilityPatcher(helper), new IClickableMenuPatcher(helper));
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "ShopMessage" })
        {
            var message = e.ReadAs<Message>();
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