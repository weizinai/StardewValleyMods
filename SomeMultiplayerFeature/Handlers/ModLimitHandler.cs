using Common;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class ModLimitHandler
{
    private const string ModRequirementPatch = "assets/Mod Requirement.json";
    
    private readonly Dictionary<string, string[]>? modRequirement;

    public ModLimitHandler(IModHelper helper)
    {
        // 读取要求模组列表
        modRequirement = helper.Data.ReadJsonFile<Dictionary<string, string[]>>(ModRequirementPatch);
        if (modRequirement is null) 
            Log.Error($"无法找到Json文件: {ModRequirementPatch}");
    }

    public void OnPeerContextReceived(PeerContextReceivedEventArgs e)
    {
        if (!Context.IsMainPlayer || !e.Peer.HasSmapi || modRequirement is null) return;

        var targetMods = e.Peer.Mods.Select(mod => mod.Name).ToList();
        var unAllowedMods = new List<string>();

        foreach (var id in modRequirement["RequiredModList"])
        {
            if (!targetMods.Contains(id))
                unAllowedMods.Add(id);
        }

        foreach (var id in targetMods)
        {
            if (!modRequirement["RequiredModList"].Contains(id) && modRequirement["AllowedModList"].Contains(id))
                unAllowedMods.Add(id);
        }
        
        if (unAllowedMods.Any())
            Game1.server.kick(e.Peer.PlayerID);
    }
}