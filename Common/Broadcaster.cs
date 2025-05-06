using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace weizinai.StardewValleyMod.Common;

internal static class Broadcaster
{
    private static string uniqueId = "";
    private static IModHelper helper = null!;
    private static readonly HashSet<string> DetectedMessageType = new() { "Info", "Alert", "NoIconHUDMessage" };

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
            var message = e.ReadAs<MessageData>();
            switch (e.Type)
            {
                case "Info":
                    Logger.Info(message.Content);
                    break;
                case "Alert":
                    Logger.Alert(message.Content);
                    break;
                case "NoIconHUDMessage":
                    Logger.NoIconHUDMessage(message.Content, message.TimeLeft);
                    break;
            }
        }
    }

    public static void Info(string message, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message), "Info", new[] { uniqueId }, playerIDs);
    }

    public static void Alert(string message, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message), "Alert", new[] { uniqueId }, playerIDs);
    }

    public static void NoIconHUDMessage(string message, float timeLeft = 3500f, long[]? playerIDs = null)
    {
        helper.Multiplayer.SendMessage(new MessageData(message, timeLeft), "NoIconHUDMessage", new[] { uniqueId }, playerIDs);
    }
}