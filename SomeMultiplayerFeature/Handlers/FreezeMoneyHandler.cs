using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

internal class FreezeMoneyHandler : BaseHandlerWithConfig<ModConfig>
{
    private const string FreezeMoneyKey = "weizinai.SomeMultiplayerFeature_FreezeMoney";

    public FreezeMoneyHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitFreezeMoneyConfig();
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
        this.InitFreezeMoneyConfig();
    }

    private void InitFreezeMoneyConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.FreezeMoney)
            modData[FreezeMoneyKey] = this.Config.FreezeMoneyAmount.ToString();
        else
            modData.Remove(FreezeMoneyKey);
    }
}