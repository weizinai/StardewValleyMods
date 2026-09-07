namespace weizinai.StardewValleyMod.AutoRefreshMineShaft.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public bool EnableMod { get; set; } = true;
}
