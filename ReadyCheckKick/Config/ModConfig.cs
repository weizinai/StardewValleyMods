using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Config;

internal class ModConfig : SingletonConfig<ModConfig>
{
    public bool ShowInfoInReadyCheckDialogue { get; set; } = true;
    public bool ShowInfoInSaveGameMenu { get; set; } = true;

    public bool AutoKickUnreadyFarmers { get; set; } = true;
    public float AutoKickUnreadyFarmersRatio { get; set; } = 0.8f;
    public int AutoKickUnreadyFarmersDelay { get; set; } = 5;
    public bool SpecialTreatForFestival { get; set; } = true;
}
