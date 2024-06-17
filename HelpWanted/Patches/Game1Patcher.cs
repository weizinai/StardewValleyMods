using Common.Patcher;
using HarmonyLib;
using StardewValley;

namespace HelpWanted.Patches;

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