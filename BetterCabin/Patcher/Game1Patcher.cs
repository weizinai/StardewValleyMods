using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            this.RequireMethod<Game1>(nameof(Game1.warpFarmer), new[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(bool) }),
            this.GetHarmonyMethod(nameof(WarpFarmerPrefix))
        );
    }

    private static bool WarpFarmerPrefix(string locationName)
    {
        if (Game1.getLocationFromName(locationName) is Cabin cabin)
        {
            var player = Game1.player;

            if (cabin.owner.Equals(player)) return true;

            if (LockCabinHandler.GetCabinWhiteList(cabin).Contains(player.Name)) return true;

            if (LockCabinHandler.CheckCabinLock(cabin))
            {
                Log.ErrorHUDMessage(I18n.UI_LockCabin_VisitLockedCabin());
                return false;
            }

            return true;
        }

        return true;
    }


}