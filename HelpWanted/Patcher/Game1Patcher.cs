using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.Patch<Game1>(harmony, nameof(Game1.RefreshQuestOfTheDay), PatchKind.Prefix, nameof(RefreshQuestOfTheDayPrefix));
    }

    // 禁止生成原版任务
    private static bool RefreshQuestOfTheDayPrefix()
    {
        return false;
    }
}
