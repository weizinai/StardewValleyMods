using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.ReadyCheckKick.Config;
using weizinai.StardewValleyMod.ReadyCheckKick.UI;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Handler;

/// <summary>
/// 过夜存盘那一面：等待其它玩家上床时，房主在右上角看到只读的未准备玩家面板。
/// 每帧按活动菜单身份幂等启停（<see cref="SaveGameMenu" /> 在即显示），数据取原版状态列表的**公共按玩家查询接口**
/// <c>PlayerStatusList.TryGetStatusText</c>，判据沿用旧实现——状态不等于 <c>ready</c>。
/// 这一面**只有房主**显示：名单对客机没有用处（踢人这件事只有房主能做），这是相对旧实现的刻意收紧。
/// 面板与准备检查那一面共用同一套视图与行组件，「只读」只体现在本处理器**不给它任何踢出动作**上。
/// </summary>
internal class SaveGameMenuHandler : BaseHandler
{
    /// <summary>面板与视口右上角之间的间隙（沿用迁移前那行红字的位置）。</summary>
    private const float CornerGap = 64f;

    /// <summary>原版状态列表里「已就绪」的状态值（<c>SaveGameMenu</c> 就写这个字面量）。</summary>
    private const string ReadyStatus = "ready";

    private readonly UnreadyFarmersOverlay overlay;

    /// <summary>本帧的未准备玩家（复用同一实例，避免每帧分配）。</summary>
    private readonly List<UnreadyFarmer> unreadyFarmers = new();

    public SaveGameMenuHandler(IModHelper helper) : base(helper)
    {
        this.overlay = new UnreadyFarmersOverlay(helper.Events.Display);
    }

    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        // 叠层宿主不订阅输入：滚轮由模组侧转给它（面板未显示时它自己什么也不做）
        this.helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        this.overlay.SyncAt(this.GetUnreadyFarmers(), new Vector2(Game1.uiViewport.Width - UnreadyFarmersPanel.Width - CornerGap, CornerGap));
    }

    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        this.overlay.HandleScrollWheel(e.Delta);
    }

    /// <summary>
    /// 取当前未准备玩家：只在房主端、过夜存盘的等待在屏幕上、且配置允许时才取，其余时刻返回空列表让面板整块隐藏。
    /// </summary>
    /// <returns>未准备玩家（复用实例）。</returns>
    private List<UnreadyFarmer> GetUnreadyFarmers()
    {
        this.unreadyFarmers.Clear();

        if (!ModConfig.Instance.ShowInfoInSaveGameMenu || !Game1.IsServer || !IsOvernightSaveWaitOnScreen())
        {
            return this.unreadyFarmers;
        }

        var endOfNightStatus = Game1.player.team.endOfNightStatus;
        var localPlayerId = Game1.player.UniqueMultiplayerID;

        foreach (var farmer in Game1.getOnlineFarmers())
        {
            // 跳过房主自己：房主此刻必然已经在过夜流程里，把他们列进「未准备」既没有可操作性（踢不了自己），
            // 也会在出货菜单那条路径上把房主自己的 shipment 状态误当成未准备显示出来
            if (farmer.UniqueMultiplayerID == localPlayerId)
            {
                continue;
            }

            if (endOfNightStatus.TryGetStatusText(farmer.UniqueMultiplayerID, out var status) && status != ReadyStatus)
            {
                this.unreadyFarmers.Add(new UnreadyFarmer(farmer.UniqueMultiplayerID, farmer.displayName, farmer));
            }
        }

        return this.unreadyFarmers;
    }

    /// <summary>
    /// 过夜存盘的等待是否正在屏幕上。两种形态都算：<see cref="SaveGameMenu" /> 自己是活动菜单（常规路径），
    /// 以及 <see cref="ShippingMenu" /> 是活动菜单——当天有东西要出货时原版走这条路径，它把
    /// <see cref="SaveGameMenu" /> **内嵌**在自己里面（<c>ShippingMenu.saveGameMenu</c>）并由它跑同一段等待，
    /// 那个内嵌菜单从不成为活动菜单。漏掉后者会让当天出过货的房主（多数夜晚）整晚看不到面板。
    /// 判据只用活动菜单身份：内嵌菜单是原版私有字段，本模组不为它重新引入反射。
    /// </summary>
    /// <returns>过夜存盘的等待在屏幕上时为 true。</returns>
    private static bool IsOvernightSaveWaitOnScreen()
    {
        return Game1.activeClickableMenu is SaveGameMenu or ShippingMenu;
    }
}
