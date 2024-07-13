using StardewValley;

namespace weizinai.StardewValleyMod.Common.Extension;

internal static class FarmerExtension
{
    public static bool IsOnline(this Farmer farmer)
    {
        return farmer.team.playerIsOnline(farmer.UniqueMultiplayerID);
    }
}