using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using weizinai.StardewValleyMod.BetterCabin.UI;
using weizinai.StardewValleyMod.PiCore;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.UI.Host;

namespace weizinai.StardewValleyMod.BetterCabin.Handler;

/// <summary>
/// 小屋世界标签的对账器：每 tick 比对当前场景「有主小屋」建筑的引用集合，为新增小屋挂一个
/// <see cref="WorldAnchorHost" />（锚点每帧求值为建筑左上角 tile，随小屋移动 / 视口滚动跟随）、
/// 为已不在场景里的小屋退订宿主，并让各小屋的内容按签名刷新标签文本。
/// 标签改由 RenderedWorld 世界锚定路径绘制，不再挂在建筑绘制补丁上（因此不受后画建筑压盖，滚出视口即裁剪）。
/// </summary>
internal class CabinTagsHandler : BaseHandler
{
    private readonly Dictionary<Building, CabinTagEntry> entries = new();

    // 每 tick 复用的比对缓冲：稳态下（场景小屋集合不变）不产生分配
    private readonly HashSet<Building> currentBuildings = new();
    private readonly List<Building> staleBuildings = new();

    public CabinTagsHandler(IModHelper helper) : base(helper) { }

    /// <inheritdoc />
    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    /// <inheritdoc />
    public override void Clear()
    {
        this.helper.Events.GameLoop.UpdateTicked -= this.OnUpdateTicked;

        this.RemoveAllEntries();
    }

    /// <summary>每 tick 对齐宿主集合，并刷新各小屋的标签文本（签名未变时内容不做任何重建）。</summary>
    /// <param name="sender">事件源（未用）。</param>
    /// <param name="e">事件数据（未用）。</param>
    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        var location = Game1.currentLocation;

        if (!Context.IsWorldReady || location is null)
        {
            this.RemoveAllEntries();

            return;
        }

        this.Reconcile(location);

        foreach (var entry in this.entries.Values)
        {
            entry.Content.Update();
        }
    }

    /// <summary>比对当前场景的小屋引用集合：新增 / 移除宿主（小屋移动不换建筑引用，宿主锚点每帧重取即可跟随）。</summary>
    /// <param name="location">当前场景。</param>
    private void Reconcile(GameLocation location)
    {
        this.currentBuildings.Clear();

        foreach (var building in location.buildings)
        {
            if (!building.IsCabinWithOwner(out var cabin)) continue;

            this.currentBuildings.Add(building);

            if (!this.entries.ContainsKey(building)) this.AddEntry(building, cabin);
        }

        // 先收集再移除（遍历字典键期间不能改动字典）；stale 通常为空，故不产生快照分配
        this.staleBuildings.Clear();

        foreach (var building in this.entries.Keys)
        {
            if (!this.currentBuildings.Contains(building)) this.staleBuildings.Add(building);
        }

        foreach (var building in this.staleBuildings)
        {
            this.RemoveEntry(building);
        }
    }

    /// <summary>为一个有主小屋挂世界锚定宿主：锚点 = 建筑左上角 tile 的绝对世界坐标，内容 = 小屋三个标签。</summary>
    /// <param name="building">该小屋的建筑。</param>
    /// <param name="cabin">该小屋（提供主人名字与在线时间）。</param>
    private void AddEntry(Building building, Cabin cabin)
    {
        var content = new CabinTagsContent(cabin);
        var host = WorldAnchorHost.Create(
            this.helper.Events.Display,
            () => PositionHelper.GetAbsolutePositionFromTilePosition(new Vector2(building.tileX.Value, building.tileY.Value)),
            content,
            WorldAnchorHost.AnchorPlacement.Centered
        );

        this.entries.Add(building, new CabinTagEntry(content, host));
    }

    /// <summary>退订该小屋的宿主并移除条目（干净移除，无残留绘制）。</summary>
    /// <param name="building">该小屋的建筑。</param>
    private void RemoveEntry(Building building)
    {
        this.entries.Remove(building, out var entry);
        entry?.Host.Disable();
    }

    /// <summary>退订全部宿主并清空比对缓冲（存档未就绪 / 处理器清理时调用，幂等）。</summary>
    private void RemoveAllEntries()
    {
        foreach (var entry in this.entries.Values)
        {
            entry.Host.Disable();
        }

        this.entries.Clear();
        this.currentBuildings.Clear();
        this.staleBuildings.Clear();
    }

    /// <summary>一个小屋的标签宿主条目：内容（标签面板）与承载它的世界锚定宿主。</summary>
    private sealed class CabinTagEntry
    {
        public CabinTagEntry(CabinTagsContent content, WorldAnchorHost host)
        {
            this.Content = content;
            this.Host = host;
        }

        /// <summary>该小屋的标签内容（对账器每 tick 调 <see cref="CabinTagsContent.Update" /> 刷新文本）。</summary>
        public CabinTagsContent Content { get; }

        /// <summary>该小屋的世界锚定宿主（<c>Disable</c> 即退订停止绘制）。</summary>
        public WorldAnchorHost Host { get; }
    }
}
