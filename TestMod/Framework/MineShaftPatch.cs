using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;

namespace TestMod.Framework;

public class MineShaftPatch
{
    public static IEnumerable<CodeInstruction> LoadLevelTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var findCreateDaySaveRandomMethod = false;
        var findChanceField = false;
        for (var i = 0; i < codes.Count; i++)
        {
            if (!findChanceField && codes[i].opcode == OpCodes.Call)
            {
                if (codes[i].opcode == OpCodes.Call && (MethodInfo)codes[i].operand == AccessTools.Method(typeof(Utility), nameof(Utility.CreateDaySaveRandom)))
                    findCreateDaySaveRandomMethod = true;
            }

            if (findCreateDaySaveRandomMethod && !findChanceField)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8 && Math.Abs((double)codes[i].operand - 0.06) < 0.1)
                {
                    codes[i].operand = 1.0;
                    findChanceField = true;
                }
            }

            if (findChanceField)
            {
                if (codes[i].opcode == OpCodes.Stloc_1)
                {
                    codes.Insert(i - 3, new CodeInstruction(OpCodes.Ldc_I4, ModEntry.Config.MapNumberToLoad));
                    codes.Insert(i - 2, new CodeInstruction(OpCodes.Stloc_0));
                    break;
                }
            }
        }

        return codes.AsEnumerable();
    }
}