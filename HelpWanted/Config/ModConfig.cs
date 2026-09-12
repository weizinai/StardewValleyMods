namespace weizinai.StardewValleyMod.HelpWanted.Config;

public class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public VanillaModConfig VanillaConfig { get; set; } = new();
    public RSVModConfig RSVConfig { get; set; } = new();

    public bool ShowQuestGenerationTooltip { get; set; } = true;

    #region 外观

    // 便签缩放
    public float NoteScale { get; set; } = 2f;

    // 便签重叠率
    public float XOverlapBoundary { get; set; } = 0.5f;
    public float YOverlapBoundary { get; set; } = 0.25f;

    // 随机颜色通道
    public int RandomColorMin { get; set; } = 150;
    public int RandomColorMax { get; set; } = 255;

    // 肖像缩放
    public float PortraitScale { get; set; } = 1f;

    // 肖像色调
    public int PortraitTintR { get; set; } = 150;
    public int PortraitTintG { get; set; } = 150;
    public int PortraitTintB { get; set; } = 150;
    public int PortraitTintA { get; set; } = 150;

    #endregion
}
