using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.TestMod.Config;

internal class ModConfig : SingletonConfig<ModConfig>
{
    // 齐瓜牌概率
    public ToggleValueConfig<float> CardChance { get; set; } = new(true, 0.005f);

    // 轮盘旋转速度
    public ToggleValueConfig<int> WheelSpinSpeed { get; set; } = new(true, 0);
    public bool ExtraSpeed { get; set; }
}
