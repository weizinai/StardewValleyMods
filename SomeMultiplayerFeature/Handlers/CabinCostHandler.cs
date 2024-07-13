using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Buildings;
using weizinai.StardewValleyMod.Common.Handler;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class CabinCostHandler : BaseHandler
{
    public CabinCostHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.Content.AssetRequested += this.OnAssetRequested;
    }

    public override void Clear()
    {
        this.Helper.Events.Content.AssetRequested -= this.OnAssetRequested;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo("Data/Buildings"))
        {
            e.Edit(asset =>
            {
                var buildingData = asset.AsDictionary<string, BuildingData>().Data;
                buildingData["Cabin"].BuildCost = 0;
            });
        }
    }
}