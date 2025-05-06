using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience.Patcher;

internal class SObjectPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<SObject>(nameof(SObject.PlaceInMachine)),
            postfix: this.GetHarmonyMethod(nameof(PlaceInMachinePostfix))
        );
    }

    // 放入机器获得经验
    private static void PlaceInMachinePostfix(SObject __instance, bool __result, Farmer who, bool probe)
    {
        if (__instance.GetMachineData() is null) return;

        if (__result == false || probe) return;

        // farming 0 | fishing 1 | foraging 2 | mining 3 | combat 4 
        switch (__instance.QualifiedItemId)
        {
            case "(BC)12":
            {
                who.gainExperience(0, 10);
                break;
            }
            // 熔炉
            case "(BC)13":
            {
                who.gainExperience(3, 3);
                break;
            }
            // 回收机
            case "(BC)20":
            {
                who.gainExperience(1, 1);
                break;
            }
            // 种子生成器
            case "(BC)25":
            {
                who.gainExperience(0, 2);
                break;
            }
            // 树液采集器
            case "(BC)105":
            {
                who.gainExperience(2, 2);
                break;
            }
            // 煤炭窑
            case "(BC)114":
            {
                who.gainExperience(2, 2);
                break;
            }
            // 熏鱼机
            case "(BC)FishSmoker":
            {
                who.gainExperience(1, 1);
                break;
            }
            // 重型熔炉
            case "(BC)HeavyFurnace)":
            {
                who.gainExperience(3, 17);
                break;
            }
        }
    }
}