using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class MuseumMenuPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<MuseumMenu>(nameof(MuseumMenu.receiveLeftClick)),
            transpiler: this.GetHarmonyMethod(nameof(ReceiveLeftClickTranspiler))
        );
    }

    private static IEnumerable<CodeInstruction> ReceiveLeftClickTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code =>
            code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(MuseumMenu), nameof(MuseumMenu.ReturnToDonatableItems))));
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MuseumMenuPatcher), nameof(GetExperienceFromDonation))));
        Log.Info("添加博物馆捐献物品获得200点采集经验功能");

        return codes.AsEnumerable();
    }

    private static void GetExperienceFromDonation()
    {
        var player = Game1.player;
        player.gainExperience(Farmer.foragingSkill, 200);
        MultiplayerLog.NoIconHUDMessage($"{player.Name}在博物馆捐献物品获得了200点采集经验", 500);
    }
}