using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>("breakStone"),
            transpiler: this.GetHarmonyMethod(nameof(BreakStoneTranspiler))
        );
    }

    // 修改采集铜矿、铁矿、金矿和铱矿获得的采矿经验为11点、12点、13点和14点
    private static IEnumerable<CodeInstruction> BreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindLastIndex(code =>
            code.opcode == OpCodes.Callvirt
            && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)))) - 1;
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatcher), nameof(GetStoneExperience))));

        return codes.AsEnumerable();
    }

    private static int GetStoneExperience(int origin, string stoneId)
    {
        origin = stoneId switch
        {
            "751" => 10, // 铜矿
            "290" => 11, // 铁矿
            "764" => 12, // 金矿
            "765" => 13, // 铱矿
            _ => origin
        };

        return origin;
    }
}