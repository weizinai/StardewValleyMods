using StardewModdingAPI;
using weizinai.StardewValleyMod.MoreExperience.Patcher;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MoreExperience;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 注册Harmony补丁
        HarmonyPatcher.Apply(
            this.ModManifest.UniqueID,
            new FarmAnimalPatcher(),
            new FarmerPatcher(),
            new GameLocationPatcher(),
            new HoeDirtPatcher(),
            new MuseumMenuPatcher(),
            new SObjectPatcher(),
            new TreePatcher()
        );
    }
}