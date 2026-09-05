using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class GameLocationPatcher : BasePatcher
{
    private static GameLocationPatcher instance = null!;
    private readonly ModConfig config;

    public GameLocationPatcher(ModConfig config)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"{nameof(GameLocationPatcher)} already initialized.");
        }

        this.config = config;
        instance = this;
    }

    public override void Apply(Harmony harmony)
    {
        this.Patch<GameLocation>(harmony, nameof(GameLocation.CheckGarbage), PatchKind.Transpiler, nameof(CheckGarbageTranspiler));
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
        return friendshipChange >= 0 ? friendshipChange : -instance.config.GarbageCanModify;
    }
}
