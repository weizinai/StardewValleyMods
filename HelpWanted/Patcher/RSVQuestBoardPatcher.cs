using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.HelpWanted.Config;
using weizinai.StardewValleyMod.HelpWanted.UI;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class RSVQuestBoardPatcher : BasePatcher
{
    /// <summary>
    /// 未加载 RSV 时不应用补丁；RSV 类型仅以字符串方式解析，禁用时不会触发其程序集加载。
    /// </summary>
    public override bool IsEnabled => ModEntry.IsRSVLoaded;

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(Type.GetType("RidgesideVillage.Questing.RSVQuestBoard,RidgesideVillage"), "draw", new[] { typeof(SpriteBatch) }),
            prefix: this.GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    // 将RSV任务菜单替换为自定义菜单
    private static bool DrawPrefix(string ___boardType)
    {
        if (___boardType != "VillageQuestBoard" || !ModConfig.Instance.RSVConfig.EnableRSVQuestBoard)
        {
            return true;
        }

        Logger<ModEntry>.Trace("Detected activation of the RSV daily quest menu. It has been replaced with the custom menu.");
        Game1.activeClickableMenu = new RSVQuestBoard();

        return false;
    }
}
