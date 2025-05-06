using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience.Patcher;

internal class MuseumMenuPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<MuseumMenu>(nameof(MuseumMenu.receiveLeftClick)),
            transpiler: this.GetHarmonyMethod(nameof(ReceiveLeftClickTranspiler))
        );
    }

    // 添加博物馆捐献获得200点采集经验
    private static IEnumerable<CodeInstruction> ReceiveLeftClickTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code =>
            code.opcode == OpCodes.Callvirt
            && code.operand.Equals(AccessTools.Method(typeof(MuseumMenu), nameof(MuseumMenu.ReturnToDonatableItems))));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MuseumMenuPatcher), nameof(GetExperienceFromDonation))));

        return codes.AsEnumerable();
    }

    private static void GetExperienceFromDonation() => Game1.player.gainExperience(Farmer.foragingSkill, 200);
}