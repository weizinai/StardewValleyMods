using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class ModLimitHandler
{
    private const string ModRequirementPatch = "assets/ModRequirement.json";

    private readonly ModConfig config;
    private readonly Dictionary<string, string[]>? modRequirement;

    public ModLimitHandler(IModHelper helper, ModConfig config)
    {
        // 初始化
        this.config = config;
        modRequirement = helper.Data.ReadJsonFile<Dictionary<string, string[]>>(ModRequirementPatch);
        if (modRequirement is null) Log.Error($"无法找到Json文件: {ModRequirementPatch}");
    }

    public void OnPeerConnected(PeerConnectedEventArgs e)
    {
        if (!Context.IsMainPlayer || !e.Peer.HasSmapi || modRequirement is null || !config.EnableModLimit) return;

        var unAllowedMods = GetUnAllowedMods(e.Peer).ToList();
        if (unAllowedMods.Any())
        {
            var detectedPlayer = Game1.getFarmer(e.Peer.PlayerID);
            Log.Info("--------------------");
            Log.Info($"{detectedPlayer.Name}已被踢出，因为其不满足模组要求：");
            foreach (var id in unAllowedMods) Log.Info(modRequirement!["RequiredModList"].Contains(id) ? $"{id}未安装" : $"{id}被禁止");
            Log.Info("--------------------");
            Game1.server.kick(e.Peer.PlayerID);
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