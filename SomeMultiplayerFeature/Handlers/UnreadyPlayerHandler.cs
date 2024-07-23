using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class UnreadyPlayerHandler : BaseHandlerWithConfig<ModConfig>
{
    private readonly HashSet<long> unreadyPlayers = new();

    public UnreadyPlayerHandler(IModHelper helper, ModConfig config)
        : base(helper, config) { }

    public override void Apply()
    {
        this.Helper.Events.Display.MenuChanged += this.OnMenuChanged;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
        this.Helper.Events.Multiplayer.PeerDisconnected += this.OnPeerDisconnected;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    public override void Clear()
    {
        this.Helper.Events.Display.MenuChanged -= this.OnMenuChanged;
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
        this.Helper.Events.Multiplayer.PeerDisconnected -= this.OnPeerDisconnected;
        this.Helper.Events.Multiplayer.ModMessageReceived -= this.OnModMessageReceived;
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (Game1.IsClient)
        {
            var message = e.NewMenu is ReadyCheckDialog ? "Ready" : "Unready";
            this.Helper.Multiplayer.SendMessage(message, "ReadyCheck",
                new[] { "weizinai.SomeMultiplayerFeature" }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!this.IsKickUnreadyPlayerEnable()) return;

        if (this.Config.KickUnreadyPlayerKey.JustPressed())
        {
            Log.Info("-- 开始踢出玩家 --");
            foreach (var player in this.unreadyPlayers)
            {
                Game1.server.kick(player);
                Log.Info($"{Game1.getFarmer(player).Name}未准备好，已被踢出。");
            }
            this.unreadyPlayers.Clear();
            Log.Info("-- 结束踢出玩家 --");
        }
    }

    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (Game1.IsServer) this.unreadyPlayers.Remove(e.Peer.PlayerID);
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (!this.IsKickUnreadyPlayerEnable()) return;

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "ReadyCheck" })
        {
            var message = e.ReadAs<string>();
            if (message is "Unready")
                this.unreadyPlayers.Add(e.FromPlayerID);
            else
                this.unreadyPlayers.Remove(e.FromPlayerID);
        }
    }

    private bool IsKickUnreadyPlayerEnable()
    {
        return Game1.IsServer && this.Config.KickUnreadyPlayer;
    }
}