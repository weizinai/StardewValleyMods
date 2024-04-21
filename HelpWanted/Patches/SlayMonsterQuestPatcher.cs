using System.Reflection.Emit;
using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using StardewValley.Quests;

namespace HelpWanted.Patches;

public class SlayMonsterQuestPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public SlayMonsterQuestPatcher(ModConfig config)
    {
        SlayMonsterQuestPatcher.config = config;
    }
    
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<SlayMonsterQuest>(nameof(SlayMonsterQuest.loadQuestInfo)),
            transpiler: GetHarmonyMethod(nameof(LoadQuestInfoTranspiler))
        );
    }
    
    private static IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        return codes.AsEnumerable();
    }
}