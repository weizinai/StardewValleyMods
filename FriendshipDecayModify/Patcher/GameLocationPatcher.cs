using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship))));
        codes.Insert(index - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatcher), nameof(GetGarbageCanModify))));

        return codes.AsEnumerable();
    }

    private static int GetGarbageCanModify(int friendshipChange)
    {
        return friendshipChange >= 0 ? friendshipChange : -config.GarbageCanModify;
    }
}