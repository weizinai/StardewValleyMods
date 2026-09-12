using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network.NetReady;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.ReadyCheckKick.Config;
using weizinai.StardewValleyMod.ReadyCheckKick.UI;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Handler;

/// <summary>
/// 准备检查那一面：房主在原版「Waiting for other players...」对话框**正上方**看到未准备玩家面板，
/// 行内可点「踢出」、面板底可点「全部踢出」（同一动作、同一条日志，不做二次确认）——这是本模组唯一的手动踢人入口。
/// 所有准备检查场景都显示（睡觉、节日开始/结束、事件门槛、电影院等），与自动踢出的适用范围一致。
/// </summary>
/// <remarks>
/// **数据来源是反射，且这是唯一途径**：原版没有按玩家查询准备状态的公共接口——
/// <c>ReadySynchronizer.GetIfExists</c> 与 <c>ServerReadyCheck.ReadyStates</c> 都是私有成员、后者所在类型还是
/// <c>internal</c>，而客机侧的 <c>ClientReadyCheck</c> 只同步 <c>NumberReady</c> / <c>NumberRequired</c>，
/// **根本没有按玩家查询的能力**。因此「只有房主能看到名单」是数据决定的，不是取舍：客机端列表恒为空、面板恒不出现。
/// <para>
/// 被踢出的玩家必须本地抑制（<see cref="kickedFarmerIds" />）：被踢者会从 <c>Game1.otherFarmers</c> 掉出，但
/// <c>Game1.GetPlayer</c> 会回退到 <c>Game1.netWorldState.Value.farmhandData</c> 把**离线农民**返回（非 null），而
/// <c>ServerReadyCheck.ReadyStates</c> 里他那条 <c>NotReady</c> 要到本次检查结束才会消失
/// （<c>ServerReadyCheck.Update</c> 只按在线农民重算计数、不清条目）。本模组每帧从状态表重建列表，不抑制的话
/// 刚踢掉的人下一帧就会带着可点的按钮重新冒出来。抑制集在检查名变化或对话框关闭时清空。
/// </para>
/// <para>
/// 手动踢人与自动踢出共用同一份列表、同一套逐人日志：面板底的「全部踢出」与自动踢出走
/// <see cref="KickUnreadyFarmers" />，行内「踢出」走 <see cref="KickOneFarmer" />。
/// **手动踢人只有面板这一条通路**：配置「在准备时显示信息」关掉 = 面板与按钮整块不出现 = 只剩自动踢出还能踢人。
/// </para>
/// <para>
/// 上面这些原版事实的核对来源（本地反编译源码，只读查阅）：
/// <c>StardewValleyModsReference/StardewValley/StardewValley.Network.NetReady/ReadySynchronizer.cs</c>（<c>GetIfExists</c>）、
/// <c>.../StardewValley.Network.NetReady.Internal/ServerReadyCheck.cs</c>（<c>ReadyStates</c> 与 <c>Update</c>）、
/// <c>.../StardewValley.Network.NetReady.Internal/ClientReadyCheck.cs</c>（只同步计数）、
/// <c>.../StardewValley/Game1.cs</c>（<c>GetPlayer</c> 的两级回退）。
/// </para>
/// </remarks>
internal class ReadyCheckDialogueHandler : BaseHandler
{
    /// <summary>面板底边与原版对话框顶边之间的固定间隙。</summary>
    private const float PanelDialogGap = 8f;

    /// <summary>
    /// 配置关掉显示时喂给面板的空列表。面板见空列表即整块隐藏，而重建列表这件事照常进行——
    /// 自动踢出用的是同一份列表，关掉显示不该改变自动踢出行为。故这只是个常量空表，绝不写入。
    /// </summary>
    private static readonly List<UnreadyFarmer> EmptyFarmers = new();

    private readonly MethodInfo getIfExistsMethod;
    private readonly FieldInfo readyStatesField;
    private readonly UnreadyFarmersOverlay overlay;

    private bool isAutoKickUnreadyFarmers;

    /// <summary>本帧的未准备玩家（复用同一实例，避免每帧分配）。</summary>
    private readonly List<UnreadyFarmer> unreadyFarmers = new();

    /// <summary>已被本模组踢出、本次检查内不再列出的玩家 id（理由见类注释）。</summary>
    private readonly HashSet<long> kickedFarmerIds = new();

    /// <summary>抑制集当前所属的检查名（检查名变化或对话框关闭即清空抑制集）。</summary>
    private string? checkName;

    public ReadyCheckDialogueHandler(IModHelper helper) : base(helper)
    {
        this.getIfExistsMethod = typeof(ReadySynchronizer).GetMethod("GetIfExists", BindingFlags.Instance | BindingFlags.NonPublic)!;
        this.readyStatesField = Assembly
            .LoadFrom("Stardew Valley.dll")
            .GetType("StardewValley.Network.NetReady.Internal.ServerReadyCheck")
            !.GetField("ReadyStates", BindingFlags.Instance | BindingFlags.NonPublic)!;
        this.overlay = new UnreadyFarmersOverlay(helper.Events.Display, new UnreadyFarmerActions(this.KickOneFarmer, this.KickUnreadyFarmers));
    }

    public override void Apply()
    {
        this.helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        // 面板是 PiCore 叠层、自身不订阅输入：交互由模组侧显式转给宿主
        this.helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        this.helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        this.UpdateUnreadyFarmers();
        this.AutoKickUnreadyFarmers();

        // 配置「在准备时显示信息」关掉 = 面板（含所有按钮）整块不出现：喂空列表让它隐藏。
        // 喂空只影响绘制：列表照常重建、自动踢出照常读它，手动踢人随之消失的后果见类注释
        var shownFarmers = ModConfig.Instance.ShowInfoInReadyCheckDialogue ? this.unreadyFarmers : EmptyFarmers;

        // 面板锚在原版对话框正上方：水平居中、底边对对话框顶边留固定间隙（锚位由叠层按面板实测尺寸反算）
        this.overlay.SyncCenteredAbove(shownFarmers, ReadyCheckDialogBounds, PanelDialogGap);
    }

    /// <summary>
    /// 左键路由：命中面板按钮即触发其点击，并把这一击**吞掉**——面板就叠在原版对话框上，不吞会与对话框自身的
    /// 命中区双触发。未命中（含面板未显示）时不消费，这一击照常下发给原版对话框。
    /// </summary>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button != SButton.MouseLeft)
        {
            return;
        }

        if (this.overlay.HandleLeftClick(Game1.getMouseX(), Game1.getMouseY()))
        {
            this.helper.Input.Suppress(e.Button);
        }
    }

    /// <summary>滚轮路由：未准备的人多到超出面板视口时用它翻列表（原版这两个菜单本来就不吃滚轮，不存在竞争）。</summary>
    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        this.overlay.HandleScrollWheel(e.Delta);
    }

    /// <summary>
    /// 踢出当前列表里的全部玩家：面板底的「全部踢出」与自动踢出共用这一条路径，两者的动作与日志逐字相同。
    /// </summary>
    private void KickUnreadyFarmers()
    {
        foreach (var farmer in this.unreadyFarmers)
        {
            this.KickAndLog(farmer);
        }

        this.unreadyFarmers.Clear();
    }

    /// <summary>行内「踢出」的入口：踢掉这一名玩家，并把他立刻从列表里摘掉（不等下一帧重建）。</summary>
    /// <param name="farmer">被踢的玩家（面板那一行给出的数据）。</param>
    private void KickOneFarmer(UnreadyFarmer farmer)
    {
        this.KickAndLog(farmer);
        this.unreadyFarmers.RemoveAll(unready => unready.Id == farmer.Id);
    }

    /// <summary>
    /// 踢出一名玩家：先写既有的逐人日志，再 <c>Game1.server.kick</c>，最后记进抑制集
    /// （理由见类注释：不记的话他下一帧会带着可点的按钮重新冒出来）。
    /// </summary>
    /// <param name="farmer">被踢的玩家（与当前列表同一份数据）。</param>
    private void KickAndLog(UnreadyFarmer farmer)
    {
        Logger<ModEntry>.Info(I18n.UI_KickUnreadyFarmer_Tooltip(farmer.DisplayName));
        Game1.server.kick(farmer.Id);
        this.kickedFarmerIds.Add(farmer.Id);
    }

    private void AutoKickUnreadyFarmers()
    {
        var config = ModConfig.Instance;

        if (!config.AutoKickUnreadyFarmers)
        {
            return;
        }

        if (!this.IsServerReady(out var menu))
        {
            return;
        }

        switch (menu.checkName)
        {
            case "festivalStart" when ModConfig.Instance.SpecialTreatForFestival:
                {
                    var festivalId = $"{Game1.currentSeason}{Game1.dayOfMonth}";

                    if (Event.tryToLoadFestivalData(festivalId, out _, out _, out _, out _, out var endTime))
                    {
                        if (Game1.timeOfDay == endTime - 50)
                        {
                            Logger<ModEntry>.Info(I18n.UI_AutoKickUnreadyFarmers_FestivalTooltip());
                            this.KickUnreadyFarmers();
                        }
                    }

                    break;
                }
            default:
                {
                    var readyPlayerRatio = (float)Game1.netReady.GetNumberReady(menu.checkName)
                                           / Game1.netReady.GetNumberRequired(menu.checkName);

                    if (readyPlayerRatio > config.AutoKickUnreadyFarmersRatio && !this.isAutoKickUnreadyFarmers)
                    {
                        this.isAutoKickUnreadyFarmers = true;
                        Logger<ModEntry>.Info(
                            I18n.UI_AutoKickUnreadyFarmers_DefaultTooltip(config.AutoKickUnreadyFarmersRatio, config.AutoKickUnreadyFarmersDelay));
                        DelayedAction.functionAfterDelay(() =>
                        {
                            if (Game1.activeClickableMenu is ReadyCheckDialog)
                            {
                                this.KickUnreadyFarmers();
                            }

                            this.isAutoKickUnreadyFarmers = false;
                        }, config.AutoKickUnreadyFarmersDelay * 1000);
                    }

                    break;
                }
        }
    }

    /// <summary>
    /// 重建本帧的未准备玩家列表：只在房主端、且活动菜单是准备检查对话框时取，其余时刻清空列表（面板随之隐藏）。
    /// </summary>
    private void UpdateUnreadyFarmers()
    {
        this.unreadyFarmers.Clear();

        if (!this.IsServerReady(out var menu))
        {
            // 对话框关闭 = 本次检查结束
            this.ClearKickedFarmerIdsIfCheckChanged(null);

            return;
        }

        // 检查名变化 = 换了一次准备检查（同一套菜单被七处检查名复用）
        this.ClearKickedFarmerIdsIfCheckChanged(menu.checkName);

        var serverReadyCheck = this.getIfExistsMethod.Invoke(Game1.netReady, new object?[] { menu.checkName });
        var rawReadyStates = (IDictionary)this.readyStatesField.GetValue(serverReadyCheck)!;

        foreach (DictionaryEntry entry in rawReadyStates)
        {
            var id = (long)entry.Key;

            // 判据沿用迁移前的字面量比较；被本模组踢出过的玩家不再回到列表里
            if (entry.Value!.ToString() != "NotReady" || this.kickedFarmerIds.Contains(id))
            {
                continue;
            }

            var farmer = Game1.GetPlayer(id);

            if (farmer is not null)
            {
                this.unreadyFarmers.Add(new UnreadyFarmer(id, farmer.displayName, farmer));
            }
            else
            {
                Logger<ModEntry>.Error($"Players with {id} id could not be found");
            }
        }
    }

    /// <summary>
    /// 维护抑制集的有效期：检查名与抑制集当前所属的检查名不一致（含对话框关闭时的 null）即清空——
    /// 换一次检查就是全新的一次，上一次被踢的人不该继续被抑制。
    /// </summary>
    /// <param name="checkName">当前检查名；对话框不在时为 null。</param>
    private void ClearKickedFarmerIdsIfCheckChanged(string? checkName)
    {
        if (this.checkName == checkName)
        {
            return;
        }

        this.checkName = checkName;
        this.kickedFarmerIds.Clear();
    }

    /// <summary>
    /// 当前准备检查对话框的矩形（水平居中与面板锚位按它算）；活动菜单不是准备检查对话框时为 null（面板随之整块隐藏）。
    /// </summary>
    /// <remarks>
    /// 直接读菜单自己的几何，而不是照公式重算：这套几何本就是原版 <c>ConfirmationDialog</c> 构造函数算好写进去的
    /// （<c>x = 视口宽/2 - 文本宽/2 - 边框</c>、<c>y = 视口高/2 - 文本高/2</c>，见
    /// <c>StardewValleyModsReference/StardewValley/StardewValley.Menus/ConfirmationDialog.cs</c>），
    /// 抄一份公式只会在原版改几何时悄悄过期。
    /// </remarks>
    private static Rectangle? ReadyCheckDialogBounds
    {
        get
        {
            if (Game1.activeClickableMenu is not ReadyCheckDialog dialog)
            {
                return null;
            }

            return new Rectangle(dialog.xPositionOnScreen, dialog.yPositionOnScreen, dialog.width, dialog.height);
        }
    }

    private bool IsServerReady([NotNullWhen(true)] out ReadyCheckDialog? menu)
    {
        if (Game1.IsServer && Game1.activeClickableMenu is ReadyCheckDialog dialog)
        {
            menu = dialog;

            return true;
        }

        menu = null;

        return false;
    }
}
