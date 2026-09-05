using StardewValley;

namespace weizinai.StardewValleyMod.PiCore.Extension;

public static class FarmerExtension
{
    public static bool IsOnline(this Farmer farmer)
    {
        return farmer.team.playerIsOnline(farmer.UniqueMultiplayerID);
    }
}