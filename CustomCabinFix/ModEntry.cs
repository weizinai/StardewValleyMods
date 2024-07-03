using CustomCabinFix.Patcher;
using StardewModdingAPI;
using weizinai.StardewValleyMod.Common.Patcher;

namespace CustomCabinFix;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        HarmonyPatcher.Apply(this, new BuildingPatcher());
    }
}