using Common.Patch;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature.Patches;

public class ChatBoxPatcher : BasePatcher
{
    private static IModHelper helper = null!;

    public ChatBoxPatcher(IModHelper helper)
    {
        ChatBoxPatcher.helper = helper;
    }

    public override void Patch(Harmony harmony)
    {
        
    }

    private static void RunCommandPrefix(ChatBox __instance, string command)
    {
        var split = ArgUtility.SplitBySpace(command);
        switch (split[0])
        {
            case "cpm":
                if (!Game1.IsMultiplayer || !Game1.IsServer) 
                    break;
                
                break;
        }
    }
}