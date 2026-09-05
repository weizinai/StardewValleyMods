using StardewModdingAPI;
using weizinai.StardewValleyMod.CustomCabinFix.Patcher;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.CustomCabinFix;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new BuildingPatcher());
    }
}