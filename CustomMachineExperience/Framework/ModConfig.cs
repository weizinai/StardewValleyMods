using System.Collections.Generic;
using StardewModdingAPI;

namespace weizinai.StardewValleyMod.CustomMachineExperience.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    public Dictionary<string, ExperienceData> MachineExperienceData { get; set; } = new();
}