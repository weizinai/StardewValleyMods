using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Game1>(nameof(Game1.RefreshQuestOfTheDay)),
            GetHarmonyMethod(nameof(RefreshQuestOfTheDayPrefix))
        );
    }

    private static bool RefreshQuestOfTheDayPrefix()
    {
        return false;
    }
}