using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

/// <summary>
/// 实现与模组限制功能相关的逻辑
/// </summary>
/// <remarks>该功能仅主机端可用</remarks>
internal class ModLimitHandler : BaseHandler
{
    private const string ModRequirementPatch = "assets/ModRequirement.json";

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
        Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果功能未启用，则返回
        if (!Config.ModLimit) return;

        // 如果当前不是联机模式或者当前玩家不是主机端，则返回
        if (!Context.IsMultiplayer || !Context.IsMainPlayer) return;

        var detectedPlayer = Game1.getFarmer(e.Peer.PlayerID);

        // 如果玩家没用安装SMAPI，则踢出该玩家并发送消息
        if (!e.Peer.HasSmapi)
        {
            Log.Alert($"{detectedPlayer.Name}已被踢出，因为其未安装SMAPI。");
            return;
        }

        // 如果玩家的模组不满足要求，则踢出该玩家并发送消息
        var unAllowedMods = GetUnAllowedMods(e.Peer).ToList();
        if (unAllowedMods.Any())
        {
            Helper.Multiplayer.SendMessage(unAllowedMods, "ModLimit",
                new[] { "weizinai.SomeMultiplayerFeature" }, new[] { e.Peer.PlayerID });
            Game1.server.kick(e.Peer.PlayerID);
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (Context.IsMainPlayer) return;

        if (e is { Type: "ModLimit", FromModID: "weizinai.SomeMultiplayerFeature" })
        {
            var message = e.ReadAs<List<string>>();
            Log.Alert($"{Game1.player.Name}已被踢出，因为其不满足模组要求：");
            foreach (var id in message) Log.Info(modRequirement!["RequiredModList"].Contains(id) ? $"{id}未安装" : $"{id}被禁止");
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