namespace HelpWanted.Framework;

public class ModConfig
{
    /// <summary>是否启用模组</summary>
    public bool ModEnabled { get; set; } = true;
    
    public bool QuestFirstDay { get; set; } = false;
    /// <summary>是否必须为村民喜欢的礼物</summary>
    public bool MustLikeItem { get; set; } = true;
    /// <summary>是否必须为村民喜爱的礼物</summary>
    public bool MustLoveItem { get; set; } = false;
    /// <summary>是否允许任务物品为工匠物品</summary>
    public bool AllowArtisanGoods { get; set; } = true;
    /// <summary>是否忽略原版任务物品限制</summary>
    public bool IgnoreVanillaItemRestriction { get; set; } = true;
    /// <summary>是否每个村民一个任务</summary>
    public bool OneQuestPerVillager { get; set; } = true;
    /// <summary>是否好感度满后不再生成任务</summary>
    public bool AvoidMaxHearts { get; set; } = true;
    /// <summary>任务物品的最大价格.若为-1,则不限制.</summary>
    public int MaxPrice { get; set; } = -1;
    /// <summary>任务时间</summary>
    public int QuestDays { get; set; } = 2;
    /// <summary>任务最大数量</summary>
    public int MaxQuests { get; set; } = 10;
    /// <summary>便签缩放</summary>
    public float NoteScale { get; set; } = 2;
    /// <summary>便签水平方向重叠率</summary>
    public float XOverlapBoundary { get; set; } = 0.5f;
    /// <summary>便签垂直方向重叠率</summary>
    public float YOverlapBoundary { get; set; } = 0.25f;
    /// <summary>便签肖像缩放</summary>
    public float PortraitScale { get; set; } = 1f;
    /// <summary>便签肖像水平偏移</summary>
    public int PortraitOffsetX { get; set; } = 32;
    /// <summary>便签肖像垂直偏移</summary>
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