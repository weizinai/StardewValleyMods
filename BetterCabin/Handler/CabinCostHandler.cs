using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class CabinCostHandler : BaseHandler
{
    public CabinCostHandler(IModHelper helper) : base(helper)
    {
        if (Context.IsWorldReady) this.SetCabinCost();
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
        this.SetCabinCost();
    }

    private void SetCabinCost()
    {
        if (!Context.IsMainPlayer) return;

        Game1.buildingData["Cabin"].BuildCost = ModConfig.Instance.CabinCost;
    }
}