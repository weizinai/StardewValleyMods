using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class PurchaseBackpackHandler : BaseHandlerWithConfig<ModConfig>
{
    public const string BanLargeBackpackKey = ModEntry.ModDataPrefix + "BanLargeBackpack";
    public const string BanDeluxeBackpackKey = ModEntry.ModDataPrefix + "BanDeluxeBackpack";

    public PurchaseBackpackHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitBackpackConfig();
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
        this.InitBackpackConfig();
    }

    private void InitBackpackConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.BanLargeBackpack)
            modData[BanLargeBackpackKey] = "true";
        else
            modData.Remove(BanLargeBackpackKey);

        if (this.Config.BanDeluxeBackpack)
            modData[BanDeluxeBackpackKey] = "true";
        else
            modData.Remove(BanDeluxeBackpackKey);
    }
}