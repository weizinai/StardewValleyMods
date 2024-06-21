using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BetterCabin.Framework;

internal class ModConfig
{
    // 拜访小屋信息
    public bool VisitCabinInfo { get; set; } = true;
    
    // 小屋主人名字标签
    public bool CabinOwnerNameTag { get; set; } = true;
    public int XOffset { get; set; }
    public int YOffset { get; set; }
    
    // 删除小屋主人
    public bool DeleteFarmhand { get; set; } = true;
    public KeybindList DeleteFarmhandKeybind { get; set; } = new(SButton.Delete);
}