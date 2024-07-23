using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmerPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Farmer>(nameof(Farmer.gainExperience)),
            prefix: this.GetHarmonyMethod(nameof(GainExperiencePrefix))
        );
        harmony.Patch(
            original: this.RequireMethod<Farmer>(nameof(Farmer.performPassoutWarp)),
            transpiler: this.GetHarmonyMethod(nameof(PerformPassoutWarpTranspiler))
        );
    }

    private static void GainExperiencePrefix(int which, ref int howMuch)
    {
        // 钓鱼经验翻倍
        if (which == Farmer.fishingSkill) howMuch *= 2;
    }

    private static IEnumerable<CodeInstruction> PerformPassoutWarpTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Stfld && code.operand.ToString()!.Contains("passOutLocation"));
        codes.Insert(index, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmerPatcher), nameof(SleepLocationHandler))));

        return codes.AsEnumerable();
    }

    private static GameLocation SleepLocationHandler(GameLocation location)
    {
        if (location is FarmHouse or IslandFarmHouse or Cellar) return location;

        Log.Info("晕倒金钱惩罚已移除");
        var player = Game1.player;
        return Utility.getHomeOfFarmer(player);
    }
}