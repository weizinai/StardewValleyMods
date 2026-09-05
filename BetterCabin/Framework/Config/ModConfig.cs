using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Config;

internal class ModConfig
{
    public static ModConfig Instance { get; set; } = null!;

    public static void Init(IModHelper helper)
    {
        Instance = helper.ReadConfig<ModConfig>();
    }

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
    public OnlineTimeConfig TotalOnlineTime { get; set; } = new(false, 0, -64, Color.Black);

    // 上次在线时间
    public OnlineTimeConfig LastOnlineTime { get; set; } = new(true, 0, 64, Color.Black);

    // 小屋面板
    public bool CabinMenu { get; set; } = true;
    public KeybindList CabinMenuKeybind { get; set; } = new(SButton.O);
    public bool BuildCabinContinually { get; set; } = true;

    // 上锁小屋
    public bool LockCabin { get; set; } = true;
    public KeybindList LockCabinKeybind { get; set; } = new(SButton.L);
    public KeybindList SetWhiteListKey { get; set; } = new(SButton.U);

    // 重置小屋
    public bool ResetCabinPlayer { get; set; } = true;
    public KeybindList ResetCabinPlayerKeybind { get; set; } = new(SButton.Delete);

    // 小屋花费
    public int CabinCost { get; set; }

    // 可穿过的邮箱
    public bool PassableMailbox { get; set; } = true;

    // 强制建造小屋
    public bool ForceBuildCabin { get; set; } = true;
}