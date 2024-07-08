using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;
using weizinai.StardewValleyMod.Common.Handler;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class MachineExperienceHandler : BaseHandler
{
    public MachineExperienceHandler(IModHelper helper) : base(helper) { }

    public override void Init()
    {
        this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Machines"))
        {
            e.Edit(asset =>
                {
                    var machineData = asset.AsDictionary<string, MachineData>().Data;
                    machineData["(BC)12"].ExperienceGainOnHarvest = "farming 20";
                    machineData["(BC)13"].ExperienceGainOnHarvest = "mining 7";
                    machineData["(BC)HeavyFurnace"].ExperienceGainOnHarvest = "mining 35";
                }
            );
        }
    }
}