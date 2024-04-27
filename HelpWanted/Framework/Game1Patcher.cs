using Common.Patch;
using HarmonyLib;
using StardewValley;

namespace HelpWanted.Framework;

public class Game1Patcher : BasePatcher
{
    public override void Patch(Harmony harmony)
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