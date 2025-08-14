using StardewModdingAPI;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

public class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

    public VanillaModConfig VanillaConfig { get; set; } = new();
    public RSVModConfig RSVConfig { get; set; } = new();

    public bool ShowQuestGenerationTooltip = true;

    #region 外观

    // 便签缩放
    public float NoteScale = 2f;

    // 便签重叠率
    public float XOverlapBoundary = 0.5f;
    public float YOverlapBoundary = 0.25f;

    // 随机颜色通道
    public int RandomColorMin = 150;
    public int RandomColorMax = 255;
    
    // 肖像缩放
    public float PortraitScale = 1f;

    // 肖像色调
    public int PortraitTintR = 150;
    public int PortraitTintG = 150;
    public int PortraitTintB = 150;
    public int PortraitTintA = 150;

    #endregion
}