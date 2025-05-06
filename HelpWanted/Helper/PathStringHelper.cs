using System.Collections.Generic;
using StardewValley.Extensions;

namespace weizinai.StardewValleyMod.HelpWanted.Helper;

public class PathStringHelper
{
    private const string BasePath = "Strings\\StringsFromCSFiles:";

    private const string GameType = "Game1";
    private const string DialogueType = "Dialogue";
    private const string ItemDeliveryQuestType = "ItemDeliveryQuest";
    private const string ResourceCollectionQuestType = "ResourceCollectionQuest";
    private const string FishingQuestType = "FishingQuest";
    private const string SlayMonsterQuestType = "SlayMonsterQuest";

    private static readonly Dictionary<string, string> PathCache = new()
    {
        ["G"] = $"{BasePath}{GameType}.cs.",
        ["D"] = $"{BasePath}{DialogueType}.cs.",
        ["I"] = $"{BasePath}{ItemDeliveryQuestType}.cs.",
        ["R"] = $"{BasePath}{ResourceCollectionQuestType}.cs.",
        ["F"] = $"{BasePath}{FishingQuestType}.cs.",
        ["S"] = $"{BasePath}{SlayMonsterQuestType}.cs."
    };

    public static string GetPathString(string type, params int[] index) => $"{PathCache[type]}{ModEntry.Random.ChooseFrom(index)}";
}