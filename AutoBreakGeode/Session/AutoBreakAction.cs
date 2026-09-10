namespace weizinai.StardewValleyMod.AutoBreakGeode.Session;

/// <summary>本帧要做的唯一一件事。</summary>
internal enum AutoBreakAction
{
    /// <summary>什么都不做：开关关着、菜单不在，或本帧让行。</summary>
    None,

    /// <summary>动画正在跑，把这一帧跑快。</summary>
    SpeedUp,

    /// <summary>点一次晶球区，砸下一颗。</summary>
    Crack
}
