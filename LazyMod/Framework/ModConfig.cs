namespace LazyMod.Framework;

public class ModConfig
{
    #region 耕种

    // 自动耕地
    public bool AutoTillDirt { get; set; } = true;
    public int AutoTillDirtRange { get; set; } = 1;
    public float StopAutoTillDirtStamina { get; set; } = 3;
    // 自动浇水
    public bool AutoWaterDirt { get; set; } = true;
    public int AutoWaterDirtRange { get; set; } = 1;
    public float StopAutoWaterDirtStamina { get; set; } = 3;
    // 自动浇水
    public bool AutoRefillWateringCan { get; set; } = true;
    public int AutoRefillWateringCanRange { get; set; } = 1;
    public bool FindWateringCanFromInventory { get; set; } = true;
    // 自动播种
    public bool AutoSeed { get; set; } = true;
    public int AutoSeedRange { get; set; } = 1;
    // 自动施肥
    public bool AutoFertilize { get; set; } = true;
    public int AutoFertilizeRange { get; set; } = 1;
    // 自动收获作物
    public bool AutoHarvestCrop { get; set; } = true;
    public int AutoHarvestCropRange { get; set; } = 1;
    public bool AutoHarvestFlower { get; set; }
    // 自动摇晃果树
    public bool AutoShakeFruitTree { get; set; } = true;
    public int AutoShakeFruitTreeRange { get; set; } = 1;
    // 自动清理枯萎作物
    public bool AutoCleanDeadCrop { get; set; } = true;
    public int AutoCleanDeadCropRange { get; set; } = 1;
    public bool FindScytheFromInventory { get; set; } = true;

    #endregion

    #region 动物
    
    // 自动抚摸
    public bool AutoPetAnimal { get; set; } = true;
    public int AutoPetAnimalRange { get; set; } = 1;
    // 自动挤奶
    public bool AutoMilkAnimal { get; set; } = true;
    public int AutoMilkAnimalRange { get; set; } = 1;
    public bool FindMilkPailFromInventory { get; set; } = true;
    // 自动剪毛
    public bool AutoShearsAnimal { get; set; } = true;
    public int AutoShearsAnimalRange { get; set; } = 1;
    public bool FindShearsFromInventory { get; set; } = true;
    // 自动打开动物门
    public bool AutoOpenAnimalDoor { get; set; } = true;
    // 自动打开栅栏门
    public bool AutoOpenFenceGate { get; set; } = true;
    public int AutoOpenFenceGateRange { get; set; } = 1;    
    // 自动抚摸宠物
    public bool AutoPetPet { get; set; } = true;
    public int AutoPetPetRange { get; set; } = 1;
    
    #endregion;

    // 自动收集煤炭
    public bool AutoCollectCoal { get; set; } = true;
    public int AutoCollectCoalRange { get; set; } = 1;
    // 自动破坏容器
    public bool AutoBreakContainer { get; set; } = true;
    public int AutoBreakContainerRange { get; set; } = 1;
    public bool FindWeaponFromInventory { get; set; } = true;
    // 自动收集奖励
    public bool AutoCollectReward { get; set; } = true;
    public int AutoCollectRewardRange { get; set; } = 1;
    
    // 其他
    // 自动挖掘远古斑点
    public bool AutoDigArtifactSpots { get; set; } = true;
    public int AutoDigArtifactSpotsRange { get; set; } = 1;
    public float StopAutoDigArtifactSpotsStamina { get; set; } = 3;
    public bool FindHoeFromInventory { get; set; } = true;
}