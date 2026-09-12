using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.PiCore.Hud;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Game1>(
            harmony,
            nameof(Game1.warpFarmer),
            PatchKind.Prefix,
            nameof(WarpFarmerPrefix),
            new[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(bool) }
        );
    }

    private static bool WarpFarmerPrefix(string locationName)
    {
        if (Game1.getLocationFromName(locationName) is Cabin cabin)
        {
            var player = Game1.player;

            if (cabin.owner.Equals(player))
            {
                return true;
            }

            if (LockCabinHandler.GetCabinWhiteList(cabin).Contains(player.Name))
            {
                return true;
            }

            if (LockCabinHandler.CheckCabinLock(cabin))
            {
                HudLogger.ErrorHUDMessage(I18n.UI_LockCabin_VisitLockedCabin());

                return false;
            }
        }

        return true;
    }
}
