using StardewModdingAPI;

namespace TestMod.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    public bool EnableChangeCardChance { get; set; } = true;
    public float CardChance { get; set; } = 0.1f;
}