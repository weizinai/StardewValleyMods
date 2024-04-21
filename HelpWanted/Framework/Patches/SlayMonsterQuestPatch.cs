using System.Reflection.Emit;
using HarmonyLib;

namespace HelpWanted.Framework.Patches;

public class SlayMonsterQuestPatch
{
    public IEnumerable<CodeInstruction> LoadQuestInfoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var config = ModEntry.Config;
        var codes = instructions.ToList();
        foreach (var code in codes)
        {
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 60) code.operand = (int)(60 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 75) code.operand = (int)(75 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 150) code.operand = (int)(150 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 85) code.operand = (int)(85 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 250) code.operand = (int)(250 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 125) code.operand = (int)(125 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 180) code.operand = (int)(180 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 350) code.operand = (int)(350 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 100) code.operand = (int)(100 * config.SlayMonstersRewardModifier);
            if (code.opcode == OpCodes.Ldc_I4_S && (int)code.operand == 120)
            {
                code.operand = (int)(120 * config.SlayMonstersRewardModifier);
                break;
            }
        }

        return codes.AsEnumerable();
    }
}