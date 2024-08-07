using StardewModdingAPI;

namespace weizinai.StardewValleyMod.TestMod.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    // 齐瓜牌概率
    public SingleValueConfig<float> CardChance { get; set; } = new(true, 0.005f);

    // 轮盘旋转速度
    public SingleValueConfig<int> WheelSpinSpeed { get; set; } = new(true, 0);
    public bool ExtraSpeed { get; set; }
}