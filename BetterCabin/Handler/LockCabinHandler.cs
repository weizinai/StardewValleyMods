using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

internal class LockCabinHandler : BaseHandler
{
    private const string LockCabinKey = ModEntry.ModDataPrefix + "LockCabin";
    private const string WhiteListKey = ModEntry.ModDataPrefix + "WhiteList";

    public LockCabinHandler(IModHelper helper) : base(helper)
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

        this.InitLockCabinConfig();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.InitLockCabinConfig();
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Game1.IsServer || !Context.IsPlayerFree) return;

        if (ModConfig.Instance.LockCabinKeybind.JustPressed())
        {
            if (!CheckLockCabinEnable())
            {
                Logger.ErrorHUDMessage(I18n.UI_LockCabin_Disable());
                return;
            }

            var cabin = Utility.getHomeOfFarmer(Game1.player) as Cabin;
            if (CheckCabinLock(cabin!))
            {
                cabin!.modData.Remove(LockCabinKey);
                Logger.NoIconHUDMessage(I18n.UI_LockCabin_Unlock());
            }
            else
            {
                cabin!.modData[LockCabinKey] = "true";
                Logger.NoIconHUDMessage(I18n.UI_LockCabin_Lock());
            }
            return;
        }

        if (ModConfig.Instance.SetWhiteListKey.JustPressed())
        {
            var cabin = Utility.getHomeOfFarmer(Game1.player) as Cabin;
            var whiteList = GetCabinWhiteList(cabin!);

            var farmhands = Game1.getAllFarmers()
                .Where(farmer => !farmer.Equals(Game1.player))
                .Select(farmer => new KeyValuePair<string, string>(farmer.Name, whiteList.Contains(farmer.Name) ? farmer.Name + " (#)" : farmer.Name));

            Game1.currentLocation.ShowPagedResponses(I18n.UI_WhiteList_ChooseFarmer(), farmhands.ToList(), value =>
            {
                if (whiteList.Contains(value))
                {
                    whiteList.Remove(value);
                    Logger.NoIconHUDMessage(I18n.UI_WhiteList_Remove(value));
                }
                else
                {
                    whiteList.Add(value);
                    Logger.NoIconHUDMessage(I18n.UI_WhiteList_Add(value));
                }

                cabin!.modData[WhiteListKey] = JsonSerializer.Serialize(whiteList);
            }, itemsPerPage: 8);
        }
    }

    private void InitLockCabinConfig()
    {
        if (Game1.IsClient) return;

        var modData = Game1.MasterPlayer.modData;
        if (ModConfig.Instance.LockCabin)
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

    public static List<string> GetCabinWhiteList(Cabin cabin)
    {
        var modData = cabin.modData;

        if (modData.TryGetValue(WhiteListKey, out var value))
        {
            return JsonSerializer.Deserialize<List<string>>(value!)!;
        }

        return new List<string>();
    }
}