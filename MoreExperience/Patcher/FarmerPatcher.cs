using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience.Patcher;

internal class FarmerPatcher : BasePatcher
{
    private static readonly string[] SkillName = { "耕种", "钓鱼", "采集", "采矿", "战斗" };

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

    // 修改钓鱼获得的钓鱼经验为原来的1.5倍
    private static void GainExperiencePrefix(int which, ref int howMuch)
    {
        // 钓鱼经验×1.5
        if (which == Farmer.fishingSkill) howMuch = (int)(howMuch * 1.5);
    }

    // 添加晕倒扣除100点五种经验
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

        var player = Game1.player;
        if (player.Level < 25)
        {
            for (var i = 0; i < 5; i++)
            {
                var experience = player.experiencePoints[i];
                var experienceDecrease = Math.Min(100, experience - GetBaseExperienceByNumber(player, i));
                player.experiencePoints[i] -= experienceDecrease;
                // Log.Info($"{SkillName[i]}技能失去了{experienceDecrease}点经验");
            }
        }

        return Utility.getHomeOfFarmer(player);
    }

    private static int GetBaseExperienceByNumber(Farmer player, int number)
    {
        var level = number switch
        {
            0 => player.farmingLevel.Value,
            1 => player.fishingLevel.Value,
            2 => player.foragingLevel.Value,
            3 => player.miningLevel.Value,
            4 => player.combatLevel.Value,
            _ => -1
        };
        return Farmer.getBaseExperienceForLevel(level);
    }
}