using System.Reflection.Emit;
using HarmonyLib;

namespace HelpWanted.Framework.Patches;

public class UtilityPatch
{
    public static IEnumerable<CodeInstruction> GetRandomItemFromSeasonTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Ldloc_1));
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetRandomItem))));
        return codes.AsEnumerable();
    }
}