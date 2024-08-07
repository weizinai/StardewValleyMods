using StardewModdingAPI;

namespace weizinai.StardewValleyMod.TestMod.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    public SingleValueConfig<float> CardChance { get; set; } = new(true, 0.005f);
}