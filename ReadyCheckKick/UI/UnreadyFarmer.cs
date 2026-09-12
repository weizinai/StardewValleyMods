using StardewValley;

namespace weizinai.StardewValleyMod.ReadyCheckKick.UI;

/// <summary>
/// 未准备玩家面板一行所需的数据。视图只认这个形状，数据来源（原版状态列表 / 准备检查内部状态）留在模组的夹具里。
/// </summary>
/// <param name="Id">玩家的唯一联机 id（列表的身份键：改名算换人，列表据此判断是否需要重建）。</param>
/// <param name="DisplayName">玩家显示名。</param>
/// <param name="Farmer">原版农民引用（行内头像的绘制来源）；查不到时为 null，该行退化成只有名字。</param>
internal readonly record struct UnreadyFarmer(long Id, string DisplayName, Farmer? Farmer);
