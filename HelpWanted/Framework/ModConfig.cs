namespace HelpWanted.Framework;

public class ModConfig
{
    /// <summary>模组是否启用</summary>
    public bool ModEnabled { get; set; } = true;
    public bool MustLikeItem { get; set; } = true;
    public bool MustLoveItem { get; set; } = false;
    public bool AllowArtisanGoods { get; set; } = true;
    public bool IgnoreVanillaItemSelection { get; set; } = true;
    /// <summary>是否每个村民一个任务</summary>
    public bool OneQuestPerVillager { get; set; } = true;
    public bool AvoidMaxHearts { get; set; } = true;
    public int MaxPrice { get; set; } = -1;
    public int QuestDays { get; set; } = 2;
    public int MaxQuests { get; set; } = 10;
    public float NoteScale { get; set; } = 2;
    public float PortraitScale { get; set; } = 1f;
    public float XOverlapBoundary { get; set; } = 0.5f;
    public float YOverlapBoundary { get; set; } = 0.25f;
    public int PortraitOffsetX { get; set; } = 32;
    public int PortraitOffsetY { get; set; } = 64;
    /// <summary>随机颜色通道最小值</summary>
    public int RandomColorMin { get; set; } = 150;
    /// <summary>随机颜色通道最大值</summary>
    public int RandomColorMax { get; set; } = 255;
    /// <summary>人物肖像色调红色通道</summary>
    public int PortraitTintR { get; set; } = 150;
    /// <summary>人物肖像色调绿色通道</summary>
    public int PortraitTintG { get; set; } = 150;
    /// <summary>人物肖像色调蓝色通道</summary>
    public int PortraitTintB { get; set; } = 150;
    /// <summary>人物肖像色调透明度</summary>
    public int PortraitTintA { get; set; } = 150;
    /// <summary>采集任务权重</summary>
    public float ResourceCollectionWeight { get; set; } = 0.08f;
    /// <summary>杀怪任务权重</summary>
    public float SlayMonstersWeight { get; set; } = 0.1f;
    /// <summary>钓鱼任务权重</summary>
    public float FishingWeight { get; set; } = 0.07f;
    /// <summary>交易任务权重</summary>
    public float ItemDeliveryWeight { get; set; } = 0.4f;
}