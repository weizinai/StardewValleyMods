namespace LazyMod.Framework;

public class ModConfig
{
    #region 自动耕地

    public bool AutoTillDirt { get; set; } = true;
    public int AutoTillDirtRange { get; set; } = 1;
    public float StopAutoTillDirtStamina { get; set; } = 3;

    #endregion

    #region 自动浇水

    public bool AutoWaterDirt { get; set; } = true;
    public int AutoWaterDirtRange { get; set; } = 1;
    public float StopAutoWaterDirtStamina { get; set; } = 3;

    #endregion

    #region 自动填充水壶

    public bool AutoRefillWateringCan { get; set; } = true;
    public int AutoRefillWateringCanRange { get; set; } = 1;
    public bool FindWateringCanFromInventory { get; set; } = true;

    #endregion

    #region 自动播种

    public bool AutoSeed { get; set; } = true;
    public int AutoSeedRange { get; set; } = 1;

    #endregion

    #region 自动施肥

    public bool AutoFertilize { get; set; } = true;
    public int AutoFertilizeRange { get; set; } = 1;

    #endregion

    #region 自动收获作物

    public bool AutoHarvestCrop { get; set; } = true;
    public int AutoHarvestCropRange { get; set; } = 1;
    public bool AutoHarvestFlower { get; set; }

    #endregion

    #region 自动挖掘远古斑点

    public bool AutoDigArtifactSpots { get; set; } = true;
    public int AutoDigArtifactSpotsRange { get; set; } = 1;
    public float StopAutoDigArtifactSpotsStamina { get; set; } = 3;
    public bool FindHoeFromInventory { get; set; } = true;

    #endregion
}