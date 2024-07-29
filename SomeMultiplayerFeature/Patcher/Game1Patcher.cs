using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

internal class Game1Patcher : BasePatcher
{
    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<Game1>("checkForEscapeKeys"),
            prefix: this.GetHarmonyMethod(nameof(CheckForEscapeKeysPrefix))
        );

        Log.Info("添加禁止取消后摇功能");
    }

    // 禁止取消后摇
    private static bool CheckForEscapeKeysPrefix()
    {
        var keyboardState = Game1.input.GetKeyboardState();

        if ((Game1.player.UsingTool || Game1.freezeControls) &&
            keyboardState.IsKeyDown(Keys.RightShift) && keyboardState.IsKeyDown(Keys.R) && keyboardState.IsKeyDown(Keys.Delete))
        {
            Log.NoIconHUDMessage("取消后摇功能已被禁用");
            return false;
        }

        return true;
    }
}