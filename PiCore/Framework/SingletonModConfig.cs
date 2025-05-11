using StardewModdingAPI;

namespace weizinai.StardewValleyMod.PiCore.Framework;

public class SingletonModConfig<TConfig> where TConfig : class, new()
{
    public static TConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<TConfig>();
    }
}