using Common.Patcher;
using HarmonyLib;
using HelpWanted.Framework;
using HelpWanted.Framework.Menu;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace HelpWanted.Patches;

internal class RSVQuestBoardPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public RSVQuestBoardPatcher(ModConfig config)
    {
        RSVQuestBoardPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            AccessTools.Method(Type.GetType("RidgesideVillage.Questing.RSVQuestBoard,RidgesideVillage"), "draw", new[] { typeof(SpriteBatch) }),
            GetHarmonyMethod(nameof(DrawPrefix))
        );
    }

    private static bool DrawPrefix(string ___boardType)
    {
        if (___boardType != "VillageQuestBoard" || !config.EnableRSVQuestBoard) return true;
        Game1.activeClickableMenu = new RSVQuestBoard(config);
        return false;
    }
}