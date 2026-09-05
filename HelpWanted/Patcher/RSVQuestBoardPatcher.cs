using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Menu;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted.Patcher;

internal class RSVQuestBoardPatcher : BasePatcher
{
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
        if (___boardType != "VillageQuestBoard" || !ModConfig.Instance.RSVConfig.EnableRSVQuestBoard) return true;

        Logger.Trace("Detected activation of the RSV daily quest menu. It has been replaced with the custom menu.");
        Game1.activeClickableMenu = new RSVQuestBoard();

        return false;
    }
}