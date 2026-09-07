namespace weizinai.StardewValleyMod.FastControlInput.Framework;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public float ActionButton { get; set; } = 1.25f;
    public float UseToolButton { get; set; } = 1.25f;
}
