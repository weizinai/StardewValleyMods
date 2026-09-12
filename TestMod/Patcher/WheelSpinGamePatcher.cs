using System;
using HarmonyLib;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.TestMod.Config;

namespace weizinai.StardewValleyMod.TestMod.Patcher;

internal class WheelSpinGamePatcher : BasePatcher
{
    /// <inheritdoc />
    public override void Apply(Harmony harmony)
    {
        this.PatchConstructor<WheelSpinGame>(harmony, PatchKind.Postfix, nameof(WheelSpinGamePostfix), new[] { typeof(int) });
    }

    private static void WheelSpinGamePostfix(ref double ___arrowRotationVelocity)
    {
        var config = ModConfig.Instance;

        if (!config.WheelSpinSpeed.IsEnabled)
        {
            return;
        }

        ___arrowRotationVelocity = Math.PI / 16
                                   + config.WheelSpinSpeed.Value * Math.PI / 256
                                   + (config.ExtraSpeed ? Math.PI / 64 : 0);
    }
}
