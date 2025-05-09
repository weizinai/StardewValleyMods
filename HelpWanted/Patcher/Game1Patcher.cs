using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Game1>(nameof(Game1.RefreshQuestOfTheDay)),
            prefix: this.GetHarmonyMethod(nameof(RefreshQuestOfTheDayPrefix))
        );
    }

    // 禁止生成原版任务
    private static bool RefreshQuestOfTheDayPrefix()
    {
        return false;
    }
}