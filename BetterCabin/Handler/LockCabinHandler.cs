using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Log;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class LockCabinHandler : BaseHandlerWithConfig<ModConfig>
{
    private static string LockCabinKey => "weizinai.BetterCabin_LockCabin";

    public LockCabinHandler(IModHelper helper, ModConfig config)
        : base(helper, config)
    {
        if (Context.IsWorldReady) this.InitLockCabinConfig();
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    public override void Clear()
    {
        this.Helper.Events.GameLoop.SaveLoaded -= this.OnSaveLoaded;
        this.Helper.Events.Input.ButtonsChanged -= this.OnButtonChanged;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitLockCabinConfig();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.IsServer || !Context.IsPlayerFree) return;

        if (this.Config.LockCabinKeybind.JustPressed())
        {
            if (!CheckLockCabinEnable())
            {
                Log.ErrorHUDMessage(I18n.UI_LockCabin_Disable());
                return;
            }

            var cabin = Utility.getHomeOfFarmer(Game1.player) as Cabin;
            if (CheckCabinLock(cabin!))
            {
                cabin!.modData.Remove(LockCabinKey);
                Log.NoIconHUDMessage(I18n.UI_LockCabin_Unlock());
            }
            else
            {
                cabin!.modData[LockCabinKey] = "true";
                Log.NoIconHUDMessage(I18n.UI_LockCabin_Lock());
            }
        }
    }

    private void InitLockCabinConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (this.Config.LockCabin)
            modData[LockCabinKey] = "true";
        else
            modData.Remove(LockCabinKey);
    }

    private static bool CheckLockCabinEnable()
    {
        return Game1.MasterPlayer.modData.ContainsKey(LockCabinKey);
    }

    public static bool CheckCabinLock(Cabin cabin)
    {
        return CheckLockCabinEnable() && cabin.modData.ContainsKey(LockCabinKey);
    }
}