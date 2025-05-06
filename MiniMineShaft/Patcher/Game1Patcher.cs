using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.MiniMineShaft.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MiniMineShaft.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Game1>(nameof(Game1.getLocationFromNameInLocationsList)),
            prefix: this.GetHarmonyMethod(nameof(GetLocationFromNameInLocationsListPrefix))
        );
    }

    private static bool GetLocationFromNameInLocationsListPrefix(ref GameLocation __result, string name)
    {
        if (MiniMine.IsMineName(name))
        {
            __result = MiniMine.GetMine(name);
            return false;
        }

        return true;
    }
}