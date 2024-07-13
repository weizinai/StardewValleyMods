using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.Common.Log;

internal static class MultiplayerLog
{
    private static string uniqueId = "";
    private static IModHelper helper = null!;
    private static readonly HashSet<string> DetectedMessageType = new() { "NoIconHUDMessage" };

    public static void Init(Mod mod)
    {
        // 初始化
        helper = mod.Helper;
        uniqueId = mod.ModManifest.UniqueID;
        // 注册事件
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    private static void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (DetectedMessageType.Contains(e.Type) && e.FromModID == uniqueId)
        {
            var message = e.ReadAs<ModMessage>();
            switch (e.Type)
            {
                case "NoIconHUDMessage":
                    Log.NoIconHUDMessage(message.Content, message.TimeLeft);
                    break;
            }
        }
    }

    public static void NoIconHUDMessage(string message, float timeLeft = 3500f)
    {
        helper.Multiplayer.SendMessage(new ModMessage(message, timeLeft), "NoIconHUDMessage", new[] { uniqueId });
    }
}