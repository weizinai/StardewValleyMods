using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Machines;
using weizinai.StardewValleyMod.CustomMachineExperience.Framework;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.CustomMachineExperience;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        ModConfig.Init(helper);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Machines"))
        {
            e.Edit(asset =>
                {
                    var machineData = asset.AsDictionary<string, MachineData>().Data;
                    foreach (var (id, data) in machineData)
                    {
                        if (ModConfig.Instance.MachineExperienceData.TryGetValue(id, out var value))
                        {
                            data.ExperienceGainOnHarvest = value.ToString();
                        }
                    }
                }
            );
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.InitConfig();

        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForMoreExperience(),
            () => ModConfig.Instance,
            config => ModConfig.Instance = config,
            this.InitConfig
        );
    }

    private void InitConfig()
    {
        var machineData = Game1.content.Load<Dictionary<string, MachineData>>("Data/Machines");
        foreach (var (id, _) in machineData)
        {
            ModConfig.Instance.MachineExperienceData.TryAdd(id, new ExperienceData());
        }
        this.Helper.WriteConfig(ModConfig.Instance);
    }
}