using System;

namespace weizinai.StardewValleyMod.ReadyCheckKick.UI;

/// <summary>
/// 面板交互形态要执行的两个踢出动作。**为 null 即只读形态**——这是两块面板（准备检查 / 过夜存盘）唯一的区别开关：
/// 没有动作时行内与面板底都不建按钮。
/// </summary>
/// <param name="KickFarmer">踢出这一名玩家（逐人写一条日志）。</param>
/// <param name="KickAllFarmers">踢出当前列出的全部玩家（不做二次确认）。</param>
internal readonly record struct UnreadyFarmerActions(Action<UnreadyFarmer> KickFarmer, Action KickAllFarmers);
