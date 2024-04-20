using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace HelpWanted.Framework.Patches;

public class UtilityPatch
{
    public static void GetRandomItemFromSeasonPrefix(ref int randomSeedAddition)
    {
        var config = ModEntry.Config;
        
        if (!config.ModEnabled || !ModEntry.GettingQuestDetails)
            return;
        randomSeedAddition += Game1.random.Next();
    }

    public static IEnumerable<CodeInstruction> GetRandomItemFromSeasonTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Ldloc_1));
        codes.Insert(codes.Count - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.GetRandomItem))));
        return codes.AsEnumerable();
    }
}