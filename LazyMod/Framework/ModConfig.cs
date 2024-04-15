namespace LazyMod.Framework;

public class ModConfig
{
    #region 耕种

    // 自动耕地
    public bool AutoTillDirt { get; set; } = true;
    public int AutoTillDirtRange { get; set; } = 1;
    public float StopAutoTillDirtStamina { get; set; } = 3;
    // 自动清理耕地
    public bool AutoClearTilledDirt { get; set; } = true;
    public int AutoClearTilledDirtRange { get; set; } = 1;
    public float StopAutoClearTilledDirtStamina { get; set; } = 3;
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
    public bool AutoClearDeadCrop { get; set; } = true;
    public int AutoClearDeadCropRange { get; set; } = 1;

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

    #region 采矿

    // 自动收集煤炭
    public bool AutoCollectCoal { get; set; } = true;
    public int AutoCollectCoalRange { get; set; } = 1;
    // 自动破坏容器
    public bool AutoBreakContainer { get; set; } = true;
    public int AutoBreakContainerRange { get; set; } = 1;
    public bool FindWeaponFromInventory { get; set; } = true;
    // 自动打开宝藏
    public bool AutoOpenTreasure { get; set; } = true;
    public int AutoOpenTreasureRange { get; set; } = 1;
    // 自动清理水晶
    public bool AutoClearCrystal { get; set; } = true;
    public int AutoClearCrystalRange { get; set; } = 1;
    // 显示梯子信息
    public bool ShowLadderInfo { get; set; } = true;
    // 显示竖井信息
    public bool ShowShaftInfo { get; set; } = true;
    // 显示怪物信息
    public bool ShowMonsterInfo { get; set; } = true;
    // 显示矿物信息
    public bool ShowMineralInfo { get; set; } = true;

    #endregion
    
    // 自动觅食
    public bool AutoForage { get; set; } = true;
    public int AutoForageRange { get; set; } = 1;
    // 自动摇树
    public bool AutoShakeTree { get; set; } = true;
    // 自动收获苔藓
    public bool AutoHarvestMoss { get; set; } = true;
    public int AutoHarvestMossRange { get; set; } = 1;
    public bool FindScytheFromInventory { get; set; } = true;
    // 自动清理树枝
    public bool AutoClearTwig { get; set; } = true;
    public int AutoClearTwigRange { get; set; } = 1;
    public float StopAutoClearTwigStamina { get; set; } = 3;
    // 自动清理树种
    public bool AutoClearTreeSeed { get; set; } = true;
    public int AutoClearTreeSeedRange { get; set; } = 1;
    public float StopAutoClearTreeSeedStamina { get; set; } = 3;
    
    // 其他
    public int AutoShakeTreeRange { get; set; } = 1;
    // 自动挖掘远古斑点
    public bool AutoDigArtifactSpots { get; set; } = true;
    public int AutoDigArtifactSpotsRange { get; set; } = 1;
    public float StopAutoDigArtifactSpotsStamina { get; set; } = 3;
    public bool FindHoeFromInventory { get; set; } = true;
}