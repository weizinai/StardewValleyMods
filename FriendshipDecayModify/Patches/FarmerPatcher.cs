using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;
using StardewValley.Characters;

namespace FriendshipDecayModify.Patches;

public class FarmerPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FarmerPatcher(ModConfig config)
    {
        FarmerPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Farmer>(nameof(Farmer.resetFriendshipsForNewDay)),
            prefix: GetHarmonyMethod(nameof(ResetFriendshipsForNewDayPrefix))
        );
    }

    private static bool ResetFriendshipsForNewDayPrefix(Farmer __instance)
    {
        foreach (var name in __instance.friendshipData.Keys)
        {
            var npc = Game1.getCharacterFromName(name) ?? Game1.getCharacterFromName<Child>(name, false);
            if (npc is null) continue;
            
            var single = npc.datable.Value && !__instance.friendshipData[name].IsDating() && !npc.isMarried();
            var talkToNPC = __instance.hasPlayerTalkedToNPC(name);
            var points = __instance.friendshipData[name].Points;

            if (talkToNPC)
            {
                __instance.friendshipData[name].TalkedToToday = false;
            }
            else
            {
                if (__instance.spouse == name)
                    __instance.changeFriendship(-config.DailyGreetingModifyForSpouse, npc);
                else if (__instance.friendshipData[name].IsDating() && points < 2500)
                    __instance.changeFriendship(-config.DailyGreetingModifyForDatingVillager, npc);
                else if ((!single && points < 2500) || (single && points < 2000))
                    __instance.changeFriendship(-config.DailyGreetingModifyForVillager, npc);
            }
        }

        __instance.updateFriendshipGifts(Game1.Date);
        return false;
    }
}