using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class HouseUpgradeHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string HouseUpgradeKey = ModEntry.ModDataPrefix + "HouseUpgrade";

    public HouseUpgradeHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitHouseUpgradeConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitHouseUpgradeConfig();
    }

    private void InitHouseUpgradeConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        var targetConfig = this.Config.BanHouseUpgrade;
        for (var i = 0; i < targetConfig.Length; i++)
        {
            if (targetConfig[i])
                modData[HouseUpgradeKey + i] = "true";
            else
                modData.Remove(HouseUpgradeKey + i);
        }
    }
}