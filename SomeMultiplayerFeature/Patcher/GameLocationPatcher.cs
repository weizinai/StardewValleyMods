using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>("breakStone"),
            transpiler: this.GetHarmonyMethod(nameof(BreakStoneTranspiler))
        );
        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction)),
            prefix: this.GetHarmonyMethod(nameof(AnswerDialogueActionPrefix))
        );
    }

    private static IEnumerable<CodeInstruction> BreakStoneTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindLastIndex(code => code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)))) - 1;
        codes.Insert(index + 1, new CodeInstruction(OpCodes.Ldarg_1));
        codes.Insert(index + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatcher), nameof(GetStoneExperience))));

        return codes.AsEnumerable();
    }

    private static bool AnswerDialogueActionPrefix(string questionAndAnswer)
    {
        if (Game1.IsServer) return true;

        if (questionAndAnswer == "Backpack_Purchase")
        {
            var modData = Game1.MasterPlayer.modData;
            switch (Game1.player.MaxItems)
            {
                case 12 when modData.ContainsKey(PurchaseBackpackHandler.BanLargeBackpackKey):
                case 24 when modData.ContainsKey(PurchaseBackpackHandler.BanDeluxeBackpackKey):
                    Game1.drawObjectDialogue("购买背包已被禁止");
                    return false;
            }
        }

        return true;
    }

    private static int GetStoneExperience(int origin, string stoneId)
    {
        origin = stoneId switch
        {
            "751" => 14, // 铜矿
            "290" => 16, // 铁矿
            "764" => 18, // 金矿
            "765" => 20, // 铱矿
            _ => origin
        };

        return Game1.player.mailReceived.Contains("gotMasteryHint") ? origin / 2 : origin;
    }
}