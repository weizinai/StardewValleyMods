namespace TestMod.Framework;

public class ModConfig
{
    public int MineShaftMap { get; set; } = 40;
    public int VolcanoDungeonMap { get; set; } = 46;
    
    // 显示梯子信息
    public bool ShowLadderInfo { get; set; } = true;
    // 显示竖井信息
    public bool ShowShaftInfo { get; set; } = true;
    // 显示怪物信息
    public bool ShowMonsterInfo { get; set; } = true;
    // 显示矿物信息
    public bool ShowMineralInfo { get; set; } = true;
}