using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public GameLocationPatcher(ModConfig config)
    {
        GameLocationPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: this.RequireMethod<GameLocation>(nameof(GameLocation.CheckGarbage)),
            transpiler: this.GetHarmonyMethod(nameof(CheckGarbageTranspiler))
        );
    }

    // 垃圾桶修改
    private static IEnumerable<CodeInstruction> CheckGarbageTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeMatcher = new CodeMatcher(instructions);

        codeMatcher
            .MatchStartForward(new CodeMatch(CodeInstruction.Call(typeof(Farmer), nameof(Farmer.changeFriendship))))
            .Advance(-1)
            .Insert(CodeInstruction.Call(typeof(GameLocationPatcher), nameof(GetGarbageCanModify)));

        return codeMatcher.Instructions();
    }

    private static int GetGarbageCanModify(int friendshipChange)
    {
        return friendshipChange >= 0 ? friendshipChange : -config.GarbageCanModify;
    }
}