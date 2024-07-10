using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class FarmAnimalPatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet)),
            prefix: this.GetHarmonyMethod(nameof(PetPrefix))
        );
    }

    // 修改抚摸动物获得的经验为50点
    private static void PetPrefix(Farmer who, bool is_auto_pet, FarmAnimal __instance)
    {
        if (!__instance.wasPet.Value && !is_auto_pet)
        {
            who.gainExperience(Farmer.farmingSkill, 45);
        }
    }
}