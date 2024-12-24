using HarmonyLib;
using StardewValley.Quests;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class QuestPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Quest>("CreateInitializationRandom"),
            postfix: this.GetHarmonyMethod(nameof(CreateInitializationRandomPostfix))
        );
    }

    private static void CreateInitializationRandomPostfix(ref Random __result)
    {
        __result = new Random();
    }
}