using StardewModdingAPI;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    public bool ShowInfoInReadyCheckDialogue { get; set; } = true;
    public bool ShowInfoInSaveGameMenu { get; set; } = true;

    public bool AutoKickUnreadyFarmers { get; set; } = true;
    public float AutoKickUnreadyFarmersRatio { get; set; } = 0.8f;
    public int AutoKickUnreadyFarmersDelay { get; set; } = 5;
    public bool SpecialTreatForFestival { get; set; } = true;
}