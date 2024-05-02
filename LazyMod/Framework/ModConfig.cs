using LazyMod.Framework.Automation;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace LazyMod.Framework;

public class ModConfig
{
    #region 耕种

    // 自动耕地
    public bool AutoTillDirt { get; set; }
    public int AutoTillDirtRange { get; set; }
    public float StopTillDirtStamina { get; set; } = 3;
    // 自动清理耕地
    public bool AutoClearTilledDirt { get; set; }
    public int AutoClearTilledDirtRange { get; set; }
    public float StopClearTilledDirtStamina { get; set; } = 3;
    // 自动浇水
    public bool AutoWaterDirt { get; set; }
    public int AutoWaterDirtRange { get; set; }
    public float StopWaterDirtStamina { get; set; } = 3;
    // 自动补充水壶
    public bool AutoRefillWateringCan { get; set; }
    public int AutoRefillWateringCanRange { get; set; } = 1;
    public bool FindWateringCanFromInventory { get; set; } = true;
    // 自动播种
    public bool AutoSeed { get; set; }
    public int AutoSeedRange { get; set; }
    // 自动施肥
    public bool AutoFertilize { get; set; }
    public int AutoFertilizeRange { get; set; }
    // 自动收获作物
    public bool AutoHarvestCrop { get; set; }
    public int AutoHarvestCropRange { get; set; } = 1;
    public bool AutoHarvestFlower { get; set; }
    // 自动摇晃果树
    public bool AutoShakeFruitTree { get; set; }
    public int AutoShakeFruitTreeRange { get; set; } = 1;
    // 自动清理枯萎作物
    public bool AutoClearDeadCrop { get; set; }
    public int AutoClearDeadCropRange { get; set; } = 1;

    #endregion

    #region 动物
    
    // 自动抚摸动物
    public bool AutoPetAnimal { get; set; }
    public int AutoPetAnimalRange { get; set; } = 1;
    // 自动挤奶
    public bool AutoMilkAnimal { get; set; }
    public int AutoMilkAnimalRange { get; set; } = 1;
    public float StopMilkAnimalStamina { get; set; } = 3;
    public bool FindMilkPailFromInventory { get; set; } = true;
    // 自动剪毛
    public bool AutoShearsAnimal { get; set; }
    public int AutoShearsAnimalRange { get; set; } = 1;
    public float StopShearsAnimalStamina { get; set; } = 3;
    public bool FindShearsFromInventory { get; set; } = true;
    // 自动打开动物门
    public bool AutoOpenAnimalDoor { get; set; }
    // 自动打开栅栏门
    public bool AutoOpenFenceGate { get; set; }
    public int AutoOpenFenceGateRange { get; set; } = 1;    
    // 自动抚摸宠物
    public bool AutoPetPet { get; set; }
    public int AutoPetPetRange { get; set; } = 1;
    
    #endregion;

    #region 采矿
    
    // 自动清理石头
    public bool AutoClearStone { get; set; }
    public int AutoClearStoneRange { get; set; } = 1;
    public float StopClearStoneStamina { get; set; } = 3;
    public bool FindPickaxeFromInventory { get; set; } = true;
    public bool ClearStoneOnMineShaft { get; set; }
    public bool ClearStoneOnVolcano { get; set; }
    public bool ClearFarmStone { get; set; } = true;
    public bool ClearOtherStone { get; set; } = true;
    public bool ClearIslandStone { get; set; } = true;
    public bool ClearOreStone { get; set; } = true;
    public bool ClearGemStone { get; set; } = true;
    public bool ClearGeodeStone { get; set; } = true;
    
    // 自动收集煤炭
    public bool AutoCollectCoal { get; set; }
    public int AutoCollectCoalRange { get; set; } = 1;
    // 自动破坏容器
    public bool AutoBreakContainer { get; set; }
    public int AutoBreakContainerRange { get; set; } = 1;
    public bool FindWeaponFromInventory { get; set; }
    // 自动打开宝藏
    public bool AutoOpenTreasure { get; set; }
    public int AutoOpenTreasureRange { get; set; } = 1;
    // 自动清理水晶
    public bool AutoClearCrystal { get; set; }
    public int AutoClearCrystalRange { get; set; } = 1;
    public bool FindToolForClearCrystal { get; set; } = true;
    // 显示梯子信息
    public bool ShowLadderInfo { get; set; } = true;
    // 显示竖井信息
    public bool ShowShaftInfo { get; set; } = true;
    // 显示怪物信息
    public bool ShowMonsterInfo { get; set; } = true;
    public bool ShowMonsterKillInfo { get; set; } = true;
    // 显示矿物信息
    public bool ShowMineralInfo { get; set; } = true;

    #endregion

    #region 采集

    // 自动觅食
    public bool AutoForage { get; set; }
    public int AutoForageRange { get; set; } = 1;
    // 自动摇树
    public bool AutoShakeTree { get; set; }
    public int AutoShakeTreeRange { get; set; } = 1;
    // 自动收获苔藓
    public bool AutoHarvestMoss { get; set; }
    public int AutoHarvestMossRange { get; set; } = 1;
    public bool FindScytheFromInventory { get; set; } = true;
    // 自动放置采集器
    public bool AutoPlaceTapper { get; set; }
    public int AutoPlaceTapperRange { get; set; } = 1;
    // 自动放置醋
    public bool AutoPlaceVinegar { get; set; }
    public int AutoPlaceVinegarRange { get; set; } = 1;
    // 自动清理树枝
    public bool AutoClearTwig { get; set; }
    public int AutoClearTwigRange { get; set; } = 1;
    public float StopClearTwigStamina { get; set; } = 3;
    public bool FindAxeFromInventory { get; set; } = true;
    // 自动清理树种
    public bool AutoClearTreeSeed { get; set; }
    public int AutoClearTreeSeedRange { get; set; }
    public float StopClearTreeSeedStamina { get; set; } = 3;

    #endregion

    #region 钓鱼

    // 自动放置蟹笼
    public bool AutoPlaceCarbPot { get; set; }
    public int AutoPlaceCarbPotRange { get; set; } = 1;
    // 自动添加鱼饵鱼饵
    public bool AutoAddBaitForCarbPot { get; set; }
    public int AutoAddBaitForCarbPotRange { get; set; } = 1;
    // 自动收获蟹笼
    public bool AutoHarvestCarbPot { get; set; }
    public int AutoHarvestCarbPotRange { get; set; } = 1;

    #endregion

    #region 食物

    // 自动吃食物_体力
    public bool AutoEatFoodForStamina { get; set; }
    public float AutoEatFoodStaminaRate { get; set; } = 0.1f;
    public bool IntelligentFoodSelectionForStamina { get; set; } = true;

    // 自动吃食物_生命值
    public bool AutoEatFoodForHealth { get; set; }
    public float AutoEatFoodHealthRate { get; set; } = 0.1f;
    public bool IntelligentFoodSelectionForHealth { get; set; } = true;
    // 自动吃增益食物
    public bool AutoEatBuffFood { get; set; }
    public BuffType FoodBuffMaintain1 { get; set; } = BuffType.Speed;
    public BuffType FoodBuffMaintain2 { get; set; } = BuffType.None;
    
    // 自动喝增益饮料
    public bool AutoDrinkBuffDrink { get; set; }
    public BuffType DrinkBuffMaintain1 { get; set; } = BuffType.Speed;
    public BuffType DrinkBuffMaintain2 { get; set; } = BuffType.None;

    #endregion

    #region 其他
    
    // 磁力半径增加
    public int MagneticRadiusIncrease { get; set; } = 64;
    // 自动清理杂草
    public bool AutoClearWeeds { get; set; }
    public int AutoClearWeedsRange { get; set; } = 1;
    public bool FindToolFromInventory { get; set; } = true;
    // 自动挖掘远古斑点
    public bool AutoDigSpots { get; set; }
    public int AutoDigSpotsRange { get; set; }
    public float StopDigSpotsStamina { get; set; } = 3;
    public bool FindHoeFromInventory { get; set; } = true;
    // 自动收获机器
    public bool AutoHarvestMachine { get; set; }
    public int AutoHarvestMachineRange { get; set; } = 1;
    // 自动触发机器
    public bool AutoTriggerMachine { get; set; }
    public int AutoTriggerMachineRange { get; set; } = 1;
    // 自动翻垃圾桶
    public bool AutoGarbageCan { get; set; }
    public int AutoGarbageCanRange { get; set; } = 1;
    public bool StopGarbageCanNearVillager { get; set; } = true;
    // 自动放置地板
    public bool AutoPlaceFloor { get; set; }
    public int AutoPlaceFloorRange { get; set; } = 1;

    #endregion
}