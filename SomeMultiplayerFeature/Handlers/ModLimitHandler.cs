using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

/// <summary>
/// 实现与模组限制功能相关的逻辑
/// </summary>
/// <remarks>该功能仅主机端可用</remarks>
internal class ModLimitHandler : BaseHandler
{
    private const string ModRequirementPatch = "assets/ModRequirement.json";

    private readonly HashSet<long> newVersionPlayers = new();
    private readonly Dictionary<string, string[]>? modRequirement;

    public ModLimitHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        modRequirement = helper.Data.ReadJsonFile<Dictionary<string, string[]>>(ModRequirementPatch);
        if (modRequirement is null)
            Log.Error($"无法找到Json文件: {ModRequirementPatch}，如果你不是主机玩家，则可以忽略该条消息。");
    }

    public override void Init()
    {
        Helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
        Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        Helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Config.EnableModLimit) return;
        
        if (newVersionPlayers.Count == Game1.otherFarmers.Count) return;

        foreach (var (id, farmer) in Game1.otherFarmers)
        {
            if (!newVersionPlayers.Contains(id))
            {
                Game1.server.kick(id);
                Log.Alert($"{farmer.Name}的SomeMultiplayerFeature模组版本不是最新版，已被踢出。");
            }
        }
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer || modRequirement is null || !Config.EnableModLimit) return;

        var detectedPlayer = Game1.getFarmer(e.Peer.PlayerID);

        // 如果玩家没用安装SMAPI，则踢出该玩家并发送消息
        if (!e.Peer.HasSmapi) Log.Alert($"{detectedPlayer.Name}已被踢出，因为其未安装SMAPI。");

        // 如果玩家的模组不满足要求，则踢出该玩家并发送消息
        var unAllowedMods = GetUnAllowedMods(e.Peer).ToList();
        if (unAllowedMods.Any())
        {
            Log.Alert($"{detectedPlayer.Name}已被踢出，因为其不满足模组要求：");
            foreach (var id in unAllowedMods) Log.Info(modRequirement!["RequiredModList"].Contains(id) ? $"{id}未安装" : $"{id}被禁止");
            Game1.server.kick(e.Peer.PlayerID);
        }
        else
        {
            // 向主机玩家发送版本验证消息
            Helper.Multiplayer.SendMessage("0.5.0", "Version",
                new[] { "weizinai.SomeMultiplayerFeature" }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
        }
    }
    
    // 当某个玩家断开连接时，将其移出<newVersionPlayers>
    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Config.EnableModLimit) return;
        newVersionPlayers.Remove(e.Peer.PlayerID);
    }

    // 如果当前玩家是房主，则接受其他玩家发送的版本验证消息。若验证通过，则将其加入<newVersionPlayers>
    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (!Context.IsMainPlayer || !Config.EnableModLimit) return;

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "Version" })
        {
            var message = e.ReadAs<string>();
            if (message is "0.5.0") newVersionPlayers.Add(e.FromPlayerID);
        }
    }

    /// <summary>
    /// 获得不满足要求的模组
    /// </summary>
    private IEnumerable<string> GetUnAllowedMods(IMultiplayerPeer peer)
    {
        var detectedMods = peer.Mods.Select(mod => mod.ID).ToList();

        foreach (var id in modRequirement!["RequiredModList"])
        {
            if (!detectedMods.Contains(id))
            {
                yield return id;
            }
        }

        foreach (var id in detectedMods)
        {
            if (!modRequirement["RequiredModList"].Contains(id) && !modRequirement["AllowedModList"].Contains(id))
            {
                yield return id;
            }
        }
    }
}