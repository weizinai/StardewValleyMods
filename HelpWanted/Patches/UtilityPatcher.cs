using System.Reflection.Emit;
using Common.Patch;
using HarmonyLib;
using StardewValley;

namespace HelpWanted.Patches;

public class UtilityPatcher : BasePatcher
{
    
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Utility>(nameof(Utility.getRandomItemFromSeason), new[] { typeof(Season), typeof(bool), typeof(Random) }),
            prefix: GetHarmonyMethod(nameof(GetRandomItemFromSeasonPrefix)),
            transpiler: GetHarmonyMethod(nameof(GetRandomItemFromSeasonTranspiler))
        );
    }

    private static bool GetRandomItemFromSeasonPrefix(ref Random random)
    {
        random = Game1.random;
        return true;
    }

    private static IEnumerable<CodeInstruction> GetRandomItemFromSeasonTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Ldloc_1));
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetRandomItem))));
        return codes.AsEnumerable();
    }
}