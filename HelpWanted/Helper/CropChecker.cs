using System.Collections.Generic;
using StardewValley;
using StardewValley.GameData.Crops;
using weizinai.StardewValleyMod.Common;

namespace weizinai.StardewValleyMod.HelpWanted.Helper;

internal static class CropChecker
{
    private static readonly Dictionary<string, CropData> CropCache = new();

    public static bool IsCrop(string itemId)
    {
        if (CropCache.ContainsKey(itemId)) return true;

        foreach (var crop in Game1.cropData.Values)
        {
            if (crop.HarvestItemId == itemId)
            {
                CropCache[itemId] = crop;

                return true;
            }
        }

        return false;
    }

    public static bool IsCurrentSeasonCrop(string itemId)
    {
        var crop = CropCache[itemId];

        if (crop.Seasons.Contains(Game1.season))
        {
            Logger.Trace($"{itemId} is a crop for the current season.");

            return true;
        }

        Logger.Trace($"{itemId} isn't a crop for the current season.");

        return false;
    }
}