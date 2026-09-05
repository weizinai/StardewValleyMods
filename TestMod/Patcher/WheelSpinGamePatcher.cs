using System;
using HarmonyLib;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Patcher;
using weizinai.StardewValleyMod.TestMod.Framework;

namespace weizinai.StardewValleyMod.TestMod.Patcher;

internal class WheelSpinGamePatcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        this.PatchConstructor<WheelSpinGame>(harmony, PatchKind.Postfix, nameof(WheelSpinGamePostfix), new[] { typeof(int) });
    }

    private static void WheelSpinGamePostfix(ref double ___arrowRotationVelocity)
    {
        var config = ModConfig.Instance.WheelSpinSpeed;

        if (!config.IsEnabled)
        {
            return;
        }

        ___arrowRotationVelocity = Math.PI / 16
                                   + config.Value * Math.PI / 256
                                   + (ModConfig.Instance.ExtraSpeed ? Math.PI / 64 : 0);
    }
}
