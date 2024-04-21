namespace HelpWanted.Framework;

public class ModConfig
{
    // 是否启用模组
    public bool ModEnabled { get; set; } = true;
    public bool QuestFirstDay { get; set; } = false;
    public bool QuestFestival { get; set; } = false;
    // 是否必须为村民喜欢的礼物
    public bool MustLikeItem { get; set; } = true;
    // 是否必须为村民喜爱的礼物
    public bool MustLoveItem { get; set; } = false;
    // 是否允许任务物品为工匠物品
    public bool AllowArtisanGoods { get; set; } = true;
    // 是否忽略原版任务物品限制
    public bool IgnoreVanillaItemRestriction { get; set; } = true;
    // 是否每个村民一个任务
    public bool OneQuestPerVillager { get; set; } = true;
    // 是否好感度满后不再生成任务
    public bool AvoidMaxHearts { get; set; } = true;
    // 任务物品的最大价格.若为-1,则不限制.
    public int MaxPrice { get; set; } = -1;
    // 任务时间
    public int QuestDays { get; set; } = 2;
    // 任务最大数量
    public int MaxQuests { get; set; } = 10;
    // 便签缩放
    public float NoteScale { get; set; } = 2;
    // 便签水平方向重叠率
    public float XOverlapBoundary { get; set; } = 0.5f;
    // 便签垂直方向重叠率
    public float YOverlapBoundary { get; set; } = 0.25f;
    // 便签肖像缩放
    public float PortraitScale { get; set; } = 1f;
    // 便签肖像水平偏移
    public int PortraitOffsetX { get; set; } = 32;
    // 便签肖像垂直偏移
    public int PortraitOffsetY { get; set; } = 64;
    // 随机颜色通道最小值
    public int RandomColorMin { get; set; } = 150;
    // 随机颜色通道最大值
    public int RandomColorMax { get; set; } = 255;
    // 人物肖像色调红色通道
    public int PortraitTintR { get; set; } = 150;
    // 人物肖像色调绿色通道
    public int PortraitTintG { get; set; } = 150;
    // 人物肖像色调蓝色通道
    public int PortraitTintB { get; set; } = 150;
    // 人物肖像色调透明度
    public int PortraitTintA { get; set; } = 150;
    // 采集任务权重
    public float ResourceCollectionWeight { get; set; } = 0.08f;
    // 杀怪任务权重
    public float SlayMonstersWeight { get; set; } = 0.1f;
    // 钓鱼任务权重
    public float FishingWeight { get; set; } = 0.07f;
    // 交易任务权重
    public float ItemDeliveryWeight { get; set; } = 0.4f;
    // 每日任务概率
    public float DailyQuestChance { get; set; } = 0.9f; 
}