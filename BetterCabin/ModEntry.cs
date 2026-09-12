using System;
using System.Collections.Generic;
using StardewModdingAPI;
using weizinai.StardewValleyMod.BetterCabin.Config;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.BetterCabin.Patcher;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin;

internal class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.BetterCabin.";

    private readonly List<IHandler> handlers = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        var configService = new ConfigService<ModConfig>(this, this.UpdateConfig);
        configService.RegisterMenu(this.BuildConfigMenu);
        this.UpdateConfig();
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this,
            new CarpenterMenuPatcher(),
            new ForceBuildCabinPatcher(),
            new Game1Patcher(),
            new PassableMailboxPatcher()
        );
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    /// <remarks>分区标题写明功能名，分区内代表“该功能总开关”的选项统一用共享的「启用」标签——分区标题与开关标签是两处独立文本，复用同一句话会让它在界面上出现两遍。</remarks>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 拜访小屋信息
            .AddSectionTitle(I18n.Config_VisitCabinInfo_Name)
            .AddBoolOption(config => config.VisitCabinInfo, I18n.Config_Enable_Name, I18n.Config_VisitCabinInfo_Tooltip)
            // 小屋主人名字标签
            .AddSectionTitle(I18n.Config_CabinOwnerNameTag_Name)
            .AddBoolOption(config => config.CabinOwnerNameTag, I18n.Config_Enable_Name, I18n.Config_CabinOwnerNameTag_Tooltip)
            .AddNumberOption(config => config.NameTagXOffset, I18n.Config_XOffset_Name)
            .AddNumberOption(config => config.NameTagYOffset, I18n.Config_YOffset_Name)
            // 总在线时间标签
            .AddSection(
                config => config.TotalOnlineTime,
                I18n.Config_TotalOnlineTimeTag_Name,
                section => ConfigureOnlineTimeSection(section, I18n.Config_TotalOnlineTimeTag_Tooltip)
            )
            // 上次在线时间标签
            .AddSection(
                config => config.LastOnlineTime,
                I18n.Config_LastOnlineTimeTag_Name,
                section => ConfigureOnlineTimeSection(section, I18n.Config_LastOnlineTimeTag_Tooltip)
            )
            // 小屋面板
            .AddSectionTitle(I18n.Config_CabinMenu_Name)
            .AddBoolOption(config => config.CabinMenu, I18n.Config_Enable_Name, I18n.Config_CabinMenu_Tooltip)
            .AddKeybindListOption(config => config.CabinMenuKeybind, I18n.Config_CabinMenuKeybind_Name)
            .AddBoolOption(config => config.BuildCabinContinually, I18n.Config_BuildCabinContinually_Name)
            // 上锁小屋
            .AddSectionTitle(I18n.Config_LockCabin_Name)
            .AddBoolOption(config => config.LockCabin, I18n.Config_Enable_Name, I18n.Config_LockCabin_Tooltip)
            .AddKeybindListOption(config => config.LockCabinKeybind, I18n.Config_LockCabinKeybind_Name)
            .AddKeybindListOption(config => config.SetWhiteListKey, I18n.Config_SetWhiteListKey_Name)
            // 删除小屋主人
            .AddSectionTitle(I18n.Config_ResetCabin_Name)
            .AddBoolOption(config => config.ResetCabinPlayer, I18n.Config_Enable_Name, I18n.Config_ResetCabin_Tooltip)
            .AddKeybindListOption(config => config.ResetCabinPlayerKeybind, I18n.Config_ResetCabinKeybind_Name)
            // 小屋花费
            .AddSectionTitle(I18n.Config_CabinCost_Name)
            .AddNumberOption(config => config.CabinCost, I18n.Config_CabinCost_Name)
            // 可穿过的邮箱
            .AddSectionTitle(I18n.Config_PassableMailbox_Name)
            .AddBoolOption(config => config.PassableMailbox, I18n.Config_Enable_Name)
            // 强制建造小屋
            .AddSectionTitle(I18n.Config_ForceBuildCabin_Name)
            .AddBoolOption(config => config.ForceBuildCabin, I18n.Config_Enable_Name);
    }

    /// <summary>把“在线时间”子配置渲染成一个分区：开关（沿用菜单统一的「启用」标签）+ 横/纵偏移三个选项（两个在线时间标签共用同一形状）。</summary>
    /// <param name="section">该子配置的分区构建器。</param>
    /// <param name="tooltip">开关悬停提示。</param>
    private static void ConfigureOnlineTimeSection(
        ConfigMenuSection<ModConfig, OnlineTimeConfig> section,
        Func<string> tooltip
    )
    {
        section
            .AddBoolOption(tag => tag.Enable, I18n.Config_Enable_Name, tooltip)
            .AddNumberOption(tag => tag.XOffset, I18n.Config_XOffset_Name)
            .AddNumberOption(tag => tag.YOffset, I18n.Config_YOffset_Name);
    }

    private void UpdateConfig()
    {
        var config = ModConfig.Instance;

        foreach (var handler in this.handlers)
        {
            handler.Clear();
        }

        this.handlers.Clear();

        if (config.ResetCabinPlayer)
        {
            this.handlers.Add(new ResetCabinHandler(this.Helper));
        }

        if (config.CabinMenu)
        {
            this.handlers.Add(new CabinMenuHandler(this.Helper));
        }

        if (config.VisitCabinInfo)
        {
            this.handlers.Add(new VisitCabinInfoHandler(this.Helper));
        }

        // 三个世界标签：各标签自己的开关在内容里判定，故对账器常驻
        this.handlers.Add(new CabinTagsHandler(this.Helper));

        if (config.LockCabin)
        {
            this.handlers.Add(new LockCabinHandler(this.Helper));
        }

        this.handlers.Add(new CabinCostHandler(this.Helper));

        foreach (var handler in this.handlers)
        {
            handler.Apply();
        }
    }
}
