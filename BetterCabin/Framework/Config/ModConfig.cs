using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BetterCabin.Framework.Config;

internal class ModConfig
{
    // 拜访小屋信息
    public bool VisitCabinInfo { get; set; } = true;
    
    // 小屋主人名字标签
    public bool CabinOwnerNameTag { get; set; } = true;
    public int NameTagXOffset { get; set; }
    public int NameTagYOffset { get; set; }
    public Color OnlineFarmerColor { get; set; } = Color.Black;
    public Color OfflineFarmerColor { get; set; } = Color.White;
    public Color OwnerColor { get; set; } = Color.Red;
    
    // 总在线时间
    public OnlineTimeConfig TotalOnlineTime { get; set; } = new(false, 0, -80, Color.Black);
    
    // 上次在线时间
    public OnlineTimeConfig LastOnlineTime { get; set; } = new(true, 0, 80, Color.Black);
    
    // 删除小屋主人
    public bool DeleteFarmhand { get; set; } = true;
    public KeybindList DeleteFarmhandKeybind { get; set; } = new(SButton.Delete);
}