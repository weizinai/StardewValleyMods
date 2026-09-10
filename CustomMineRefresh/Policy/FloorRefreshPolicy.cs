using System;
using System.Collections.Generic;
using System.Linq;

namespace weizinai.StardewValleyMod.CustomMineRefresh.Policy;

/// <summary>
/// 楼层清理策略：在一段层号区间里挑出「没有玩家」且不在保护名单里的楼层。
/// </summary>
/// <remarks>
/// 纯逻辑，不引用任何游戏类型：输入是各楼层的标识、层号与占用状态，输出是本次要移除的楼层标识集合。
/// 矿井与火山共用这一个策略，差别只有构造时的层号区间。
/// </remarks>
internal sealed class FloorRefreshPolicy
{
    /// <summary>一层在清理判定里需要的全部事实。</summary>
    /// <param name="Name">楼层标识（<c>NameOrUniqueName</c>），也是策略输出的标识。</param>
    /// <param name="Level">层号。</param>
    /// <param name="IsOccupied">该层此刻是否有玩家。</param>
    public record FloorSnapshot(string Name, int Level, bool IsOccupied);

    /// <summary>层号下限（含）。</summary>
    private readonly int minLevel;

    /// <summary>层号上限（含）。</summary>
    private readonly int maxLevel;

    /// <summary>构造策略：给定作用的层号区间。</summary>
    /// <param name="minLevel">层号下限（含）。</param>
    /// <param name="maxLevel">层号上限（含）。</param>
    public FloorRefreshPolicy(int minLevel, int maxLevel)
    {
        this.minLevel = minLevel;
        this.maxLevel = maxLevel;
    }

    /// <summary>挑出本次该移除的楼层。</summary>
    /// <param name="floors">待判定的楼层。</param>
    /// <param name="protectedNames">不得清理的楼层标识（当天断线玩家所留的地点、本机正在请求加载的地点）。</param>
    /// <param name="hasUnknownPositions">是否有在线玩家的位置此刻读不到；为 <c>true</c> 时本轮一层都不清。</param>
    /// <returns>该移除的楼层标识（<c>NameOrUniqueName</c>）。</returns>
    public IReadOnlyList<string> SelectFloorsToRemove(
        IEnumerable<FloorSnapshot> floors,
        IReadOnlySet<string> protectedNames,
        bool hasUnknownPositions
    )
    {
        // 读不到位置不等于「已经离开」：远程玩家正在进层的那一两帧正是这样，凭它清层会把人正要进的那层端掉
        if (hasUnknownPositions) return Array.Empty<string>();

        return floors.Where(floor => this.ShouldRemove(floor, protectedNames)).Select(floor => floor.Name).ToList();
    }

    /// <summary>一层该不该清：在区间内、没有玩家，且不在保护名单里。</summary>
    /// <param name="floor">待判定的楼层。</param>
    /// <param name="protectedNames">不得清理的楼层标识。</param>
    private bool ShouldRemove(FloorSnapshot floor, IReadOnlySet<string> protectedNames)
    {
        return floor.Level >= this.minLevel && floor.Level <= this.maxLevel && !floor.IsOccupied && !protectedNames.Contains(floor.Name);
    }
}
