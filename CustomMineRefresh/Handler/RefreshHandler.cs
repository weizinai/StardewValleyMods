using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.CustomMineRefresh.Config;
using weizinai.StardewValleyMod.CustomMineRefresh.Policy;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.CustomMineRefresh.Handler;

/// <summary>
/// 刷新接线：订阅两档节拍的驱动事件，把游戏对象映射成策略的纯输入，再把策略选中的楼层施加移除。
/// </summary>
/// <remarks>
/// 本类不含任何判定：要清哪些层由 <see cref="FloorRefreshPolicy" /> 说了算，什么时候该扫由 <see cref="RefreshSchedule" /> 说了算。
/// 矿井与火山共用同一条接线，差别只有层号区间与读哪一组配置；两边的开关、节拍与门控都每次现读，
/// 玩家在 GMCM 里改完即时生效，因此本处理器不进配置变更重建流程。
/// </remarks>
internal class RefreshHandler : BaseHandler
{
    // 矿井作用区间：1–120 层（121 层以上是骷髅洞穴的连续下潜玩法，不插手）
    private const int MinMineLevel = 1;
    private const int MaxMineLevel = 120;

    // 火山地牢作用区间：1–9 层（第 0 层是入口层，矮人商店与桥都在那里，永不清理）
    private const int MinVolcanoLevel = 1;
    private const int MaxVolcanoLevel = 9;

    private readonly FloorRefreshPolicy minePolicy = new(MinMineLevel, MaxMineLevel);
    private readonly FloorRefreshPolicy volcanoPolicy = new(MinVolcanoLevel, MaxVolcanoLevel);
    private readonly RefreshSchedule mineSchedule = new();
    private readonly RefreshSchedule volcanoSchedule = new();

    /// <summary>构造处理器。</summary>
    /// <param name="helper">模组的事件入口。</param>
    public RefreshHandler(IModHelper helper) : base(helper) { }

    /// <inheritdoc />
    /// <remarks>
    /// 两个驱动事件都挂上、由 <see cref="RefreshSchedule.IsDue" /> 按各自的节拍决定谁在本次事件里动手：
    /// 矿井与火山的节拍互相独立，「即时」与「按分钟」可能同时存在，而挂哪个事件不该随配置变化重建。
    /// </remarks>
    public override void Apply()
    {
        this.helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        this.helper.Events.GameLoop.DayStarted += this.OnDayStarted;
    }

    /// <inheritdoc />
    public override void Clear()
    {
        this.helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnOneSecondUpdateTicked;
        this.helper.Events.GameLoop.TimeChanged -= this.OnTimeChanged;
        this.helper.Events.GameLoop.DayStarted -= this.OnDayStarted;
    }

    /// <summary>每秒的检查：只驱动「即时」节拍的那套机制。</summary>
    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        this.RefreshIfDue(isClockTick: false);
    }

    /// <summary>游戏时钟跳（10 游戏分钟）：只驱动「按游戏分钟」节拍的那套机制。</summary>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        this.RefreshIfDue(isClockTick: true);
    }

    /// <summary>跨天把两档节拍的累计值归零。</summary>
    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.mineSchedule.Reset();
        this.volcanoSchedule.Reset();
    }

    /// <summary>按当前配置判定两套机制各自是否到点，到点的那套清理无人楼层。</summary>
    /// <param name="isClockTick">本次是游戏时钟跳（<c>true</c>）还是每秒的检查（<c>false</c>）。</param>
    private void RefreshIfDue(bool isClockTick)
    {
        var config = ModConfig.Instance;
        // 两套机制各读自己的开关与节拍、各自累计，互不影响：关掉一边不会牵动另一边
        var mineIsDue = CanRefresh(config.EnableMineRefresh) && this.mineSchedule.IsDue(config.MineRefreshInterval, isClockTick);
        var volcanoIsDue = CanRefresh(config.EnableVolcanoRefresh) && this.volcanoSchedule.IsDue(config.VolcanoRefreshInterval, isClockTick);

        if (mineIsDue) RemoveInactiveFloors(MineShaft.activeMines, mine => mine.mineLevel, this.minePolicy);
        if (volcanoIsDue) RemoveInactiveFloors(VolcanoDungeon.activeLevels, dungeon => dungeon.level.Value, this.volcanoPolicy);
    }

    /// <summary>清理一套机制里无人的楼层：枚举 → 映射成策略输入 → 调用策略 → 施加移除。</summary>
    /// <typeparam name="TFloor">该机制的楼层类型。</typeparam>
    /// <param name="activeFloors">该机制当前已加载的楼层，选中的会被就地摘掉。</param>
    /// <param name="getLevel">从楼层对象取出层号。</param>
    /// <param name="policy">该机制的层号区间。</param>
    private static void RemoveInactiveFloors<TFloor>(List<TFloor> activeFloors, Func<TFloor, int> getLevel, FloorRefreshPolicy policy)
        where TFloor : GameLocation
    {
        // 先按标识建索引：策略只认标识，施加移除时还要按标识找回原对象
        var candidates = activeFloors.ToDictionary(floor => floor.NameOrUniqueName);
        var floors = candidates.Select(entry => new FloorRefreshPolicy.FloorSnapshot(entry.Key, getLevel(entry.Value), entry.Value.farmers.Any()));
        var protectedNames = GetProtectedNames();
        var selectedNames = policy.SelectFloorsToRemove(floors, protectedNames, HasUnknownPositions());

        // 策略的输出已经落定，随后一边遍历它一边改活动列表是安全的
        foreach (var name in selectedNames)
        {
            var floor = candidates[name];

            // 原版的移除契约：从活动列表里摘掉之前必须自己调 OnRemoved，否则地图资源不释放
            floor.OnRemoved();
            activeFloors.Remove(floor);
        }
    }

    /// <summary>门控：判定只由房主权威端做（单人下也为真），世界已就绪，且该机制的开关开着。</summary>
    /// <param name="isEnabled">该机制自己的开关（两套机制各读各的，互相不影响）。</param>
    private static bool CanRefresh(bool isEnabled)
    {
        return Game1.IsMasterGame && Context.IsWorldReady && isEnabled;
    }

    /// <summary>本次清扫要放过的楼层标识：当天断线玩家所留的地点 + 本机正在请求加载的地点。</summary>
    private static IReadOnlySet<string> GetProtectedNames()
    {
        var names = new HashSet<string>();

        // 当天断线的玩家所留楼层不清：他当天重连回来不该发现那一层已经被换掉了（复刻原版 clearInactiveMines 的同名保护）
        foreach (var farmhand in Game1.getAllFarmhands())
        {
            if (farmhand.disconnectDay.Value == Game1.MasterPlayer.stats.DaysPlayed) names.Add(farmhand.disconnectLocation.Value);
        }

        // 正在请求加载的楼层不清：避免加载到一个已经被移除的地点
        var loadingName = Game1.locationRequest?.Location?.NameOrUniqueName;
        if (loadingName is not null) names.Add(loadingName);

        return names;
    }

    /// <summary>是否有在线玩家的位置此刻读不到（断线瞬间，或正在进层的那一两帧）。</summary>
    private static bool HasUnknownPositions()
    {
        return Game1.getOnlineFarmers().Any(farmer => farmer.currentLocation is null);
    }
}
