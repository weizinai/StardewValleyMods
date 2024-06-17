using Common.Patcher;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using TestMod.Framework;

namespace TestMod.Patches;

public class WheelSpinGamePatcher : BasePatcher
{
    private static ModConfig config = null!;

    public WheelSpinGamePatcher(ModConfig config)
    {
        WheelSpinGamePatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireConstructor<WheelSpinGame>(),
            postfix: GetHarmonyMethod(nameof(WheelSpinGamePostfix))
        );
    }

    private static void WheelSpinGamePostfix(ref double ___arrowRotationVelocity)
    {
        Game1.chatBox.addInfoMessage($"初始随机速度: {___arrowRotationVelocity}");

        ___arrowRotationVelocity = Math.PI / 16.0;
        ___arrowRotationVelocity += config.RandomInt * Math.PI / 256.0;
        if (config.RandomBool)
        {
            ___arrowRotationVelocity += Math.PI / 64.0;
        }

        Game1.chatBox.addInfoMessage($"修改后随机速度({config.RandomInt}-{config.RandomBool}): {___arrowRotationVelocity}");
    }
}