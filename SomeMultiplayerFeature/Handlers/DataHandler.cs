using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;
using weizinai.StardewValleyMod.Common.Handler;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class DataHandler : BaseHandler
{
    public DataHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    public override void Clear()
    {
        this.Helper.Events.Content.AssetRequested -= this.OnAssetRequested;
    }

    // 修改收获机器经验
    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Machines"))
        {
            e.Edit(asset =>
                {
                    var machineData = asset.AsDictionary<string, MachineData>().Data;
                    machineData["(BC)12"].ExperienceGainOnHarvest = "farming 20";          // 小桶
                    machineData["(BC)13"].ExperienceGainOnHarvest = "mining 7";            // 熔炉
                    machineData["(BC)20"].ExperienceGainOnHarvest = "fishing 2";           // 回收机
                    machineData["(BC)25"].ExperienceGainOnHarvest = "farming 4";           // 种子生成器
                    machineData["(BC)105"].ExperienceGainOnHarvest = "foraging 4";         // 树液采集器
                    machineData["(BC)114"].ExperienceGainOnHarvest = "foraging 4";         // 煤炭窑
                    machineData["(BC)FishSmoker"].ExperienceGainOnHarvest = "fishing 2";   // 熏鱼机
                    machineData["(BC)HeavyFurnace"].ExperienceGainOnHarvest = "mining 35"; // 重型熔炉
                }
            );
        }
    }
}