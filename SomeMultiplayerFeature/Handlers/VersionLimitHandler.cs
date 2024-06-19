using Common;
using SomeMultiplayerFeature.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SomeMultiplayerFeature.Handlers;

internal class VersionLimitHandler : BaseHandler
{
    private const string ModKey = "SomeMultiplayerFeature_Version";

    public VersionLimitHandler(IModHelper helper, ModConfig config) 
        : base(helper, config)
    {
    }

    public override void Init()
    {
        Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        // 如果该功能未启用，则返回
        if (!Config.VersionLimit) return;

        // 如果当前没有玩家在线或者当前玩家不是主机端，则返回
        if (!Context.HasRemotePlayers || !Context.IsMainPlayer) return;
        
        foreach (var (id, farmer) in Game1.otherFarmers)
        {
            if (!farmer.modData.ContainsKey(ModKey) || farmer.modData[ModKey] != "0.5.2")
            {
                Game1.server.kick(id);
                Log.Alert($"{farmer.Name}已被踢出，因为其SomeMultiplayerFeature模组不是最新版。");
            }
        }
    }

    // 当载入存档时，向玩家写入版本验证信息
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // 如果当前玩家是主机端，则返回
        if (Context.IsMainPlayer) return;

        var farmer = Game1.player;
        if (farmer.modData.ContainsKey(ModKey))
            farmer.modData[ModKey] = "0.5.2";
        else
            farmer.modData.Add(ModKey, "0.5.2");
    }
}