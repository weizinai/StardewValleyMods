using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.HelpWanted.Framework;
using weizinai.StardewValleyMod.HelpWanted.Manager;
using weizinai.StardewValleyMod.HelpWanted.Menu;
using weizinai.StardewValleyMod.HelpWanted.Patcher;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Logging;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.HelpWanted;

internal class ModEntry : Mod
{
    public static bool IsRSVLoaded;
    public static Random Random { get; } = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        TextureManager.Instance.Init(helper);
        IsRSVLoaded = this.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");

        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供任务生成与补丁读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);

        // 注册事件
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        // 注册Harmony补丁
        var patches = new List<IPatcher>
        {
            new BillboardPatcher(),
            new QuestPatcher(),
            new ItemDeliveryQuestPatcher(),
            new SlayMonsterQuestPatcher(),
            new ResourceCollectionQuestPatcher(),
            new FishingQuestPatcher(),
            new Game1Patcher(),
            new TownPatcher()
        };

        // RSV 未加载时由 RSVQuestBoardPatcher.IsEnabled 跳过，不在此处做条件注册
        patches.Add(new RSVQuestBoardPatcher());

        HarmonyPatcher.Apply(this, patches.ToArray());
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 主页：任务生成提示开关与三个子页面链接
            .AddBoolOption(config => config.ShowQuestGenerationTooltip, I18n.Config_ShowQuestGenerationTooltip_Name)
            .AddPageLink("VanillaConfig", I18n.Config_VanillaConfigPage_Name)
            .AddPageLink("RSVConfig", I18n.Config_RSVConfigPage_Name, enable: IsRSVLoaded)
            .AddPageLink("Appearance", I18n.Config_AppearancePage_Name)

            // 原版配置页
            .AddPage("VanillaConfig", I18n.Config_VanillaConfigPage_Name)
            .AddSectionTitle(I18n.Config_GeneralSettingsTitle_Name)
            .AddBoolOption(config => config.VanillaConfig.QuestFirstDay, I18n.Config_QuestFirstDay_Name, I18n.Config_QuestFirstDay_Tooltip)
            .AddBoolOption(config => config.VanillaConfig.QuestFestival, I18n.Config_QuestFestival_Name, I18n.Config_QuestFestival_Tooltip)
            .AddNumberOption(config => config.VanillaConfig.DailyQuestChance, I18n.Config_DailyQuestChance_Name, null, 0f, 1f, 0.05f)
            .AddBoolOption(config => config.VanillaConfig.OneQuestPerVillager, I18n.Config_OneQuestPerVillager_Name)
            .AddBoolOption(config => config.VanillaConfig.ExcludeMaxHeartsNPC, I18n.Config_ExcludeMaxHeartsNPC_Name)
            // 逃生舱：ExcludeNPCList 是 List<string>，以逗号分隔文本编辑（描述器未内置 List<string> 绑定）
            .AddCustomSection(configMenu => configMenu.AddTextOption(
                config => string.Join(", ", config.VanillaConfig.ExcludeNPCList),
                (config, value) => config.VanillaConfig.ExcludeNPCList = value.Split(',').Select(s => s.Trim()).ToList(),
                I18n.Config_ExcludeNPCList_Name,
                I18n.Config_ExcludeNPCList_Tooltip
            ))
            .AddNumberOption(config => config.VanillaConfig.MaxQuests, I18n.Config_MaxQuests_Name)
            // 交易任务
            .AddSection(config => config.VanillaConfig.ItemDeliveryQuestConfig, I18n.Config_ItemDeliveryQuestTitle_Name, ConfigureBaseQuestSection)
            .AddNumberOption(config => config.VanillaConfig.ItemDeliveryFriendshipGain, I18n.Config_ItemDeliveryFriendshipGain_Name)
            .AddBoolOption(config => config.VanillaConfig.RewriteQuestItem, I18n.Config_RewriteQuestItem_Name, I18n.Config_RewriteQuestItem_Tooltip)
            .AddNumberOption(
                config => config.VanillaConfig.QuestItemRequirement,
                I18n.Config_ItemGiftTasteRequirement_Name,
                null,
                0,
                4,
                1,
                value => value switch
                {
                    0 => I18n.Config_ItemGiftTasteRequirement_Love(),
                    1 => I18n.Config_ItemGiftTasteRequirement_Like(),
                    2 => I18n.Config_ItemGiftTasteRequirement_Neutral(),
                    3 => I18n.Config_ItemGiftTasteRequirement_Dislike(),
                    4 => I18n.Config_ItemGiftTasteRequirement_Hate(),
                    _ => value.ToString()
                }
            )
            .AddBoolOption(config => config.VanillaConfig.AllowArtisanGoods, I18n.Config_AllowArtisanGoods_Name, I18n.Config_AllowArtisanGoods_Tooltip)
            .AddNumberOption(config => config.VanillaConfig.MaxPrice, I18n.Config_MaxPrice_Name, I18n.Config_MaxPrice_Tooltip)
            // 采集任务
            .AddSection(config => config.VanillaConfig.ResourceCollectionQuestConfig, I18n.Config_ResourceCollectionQuestTitle_Name, ConfigureBaseQuestSection)
            .AddBoolOption(config => config.VanillaConfig.MoreResourceCollectionQuest, I18n.Config_MoreResourceCollectionQuest_Name)
            // 钓鱼任务
            .AddSection(config => config.VanillaConfig.FishingQuestConfig, I18n.Config_FishingQuestTitle_Name, ConfigureBaseQuestSection)
            // 杀怪任务
            .AddSection(config => config.VanillaConfig.SlayMonsterQuestConfig, I18n.Config_SlayMonstersQuestTitle_Name, ConfigureBaseQuestSection)
            .AddBoolOption(config => config.VanillaConfig.MoreSlayMonsterQuest, I18n.Config_MoreSlayMonsterQuests_Name)

            // RSV 配置页
            .AddPage("RSVConfig", I18n.Config_RSVConfigPage_Name)
            .AddSectionTitle(I18n.Config_GeneralSettingsTitle_Name)
            .AddBoolOption(config => config.RSVConfig.EnableRSVQuestBoard, I18n.Config_EnableRSVQuestBoard_Name)
            .AddBoolOption(config => config.RSVConfig.QuestFirstDay, I18n.Config_QuestFirstDay_Name, I18n.Config_QuestFirstDay_Tooltip)
            .AddBoolOption(config => config.RSVConfig.QuestFestival, I18n.Config_QuestFestival_Name, I18n.Config_QuestFestival_Tooltip)
            .AddNumberOption(config => config.RSVConfig.DailyQuestChance, I18n.Config_DailyQuestChance_Name, null, 0f, 1f, 0.05f)
            .AddNumberOption(config => config.RSVConfig.MaxQuests, I18n.Config_MaxQuests_Name)
            .AddBoolOption(config => config.RSVConfig.AllowSameQuest, I18n.Config_AllowSameQuest_Name)
            // 交易任务
            .AddSection(config => config.RSVConfig.ItemDeliveryQuestConfig, I18n.Config_ItemDeliveryQuestTitle_Name, ConfigureBaseQuestSection)
            // 钓鱼任务
            .AddSection(config => config.RSVConfig.FishingQuestConfig, I18n.Config_FishingQuestTitle_Name, ConfigureBaseQuestSection)
            // 杀怪任务
            .AddSection(config => config.RSVConfig.SlayMonsterQuestConfig, I18n.Config_SlayMonstersQuestTitle_Name, ConfigureBaseQuestSection)
            // 丢失物品任务
            .AddSection(config => config.RSVConfig.LostItemQuestConfig, I18n.Config_LostItemQuestTitle_Name, ConfigureBaseQuestSection)

            // 外观页
            .AddPage("Appearance", I18n.Config_AppearancePage_Name)
            // 便签外观
            .AddSectionTitle(I18n.Config_NoteAppearanceTitle_Name)
            .AddNumberOption(config => config.NoteScale, I18n.Config_NoteScale_Name)
            .AddNumberOption(config => config.XOverlapBoundary, I18n.Config_XOverlapBoundary_Name, I18n.Config_XOverlapBoundary_Tooltip, 0f, 1f, 0.05f)
            .AddNumberOption(config => config.YOverlapBoundary, I18n.Config_YOverlapBoundary_Name, I18n.Config_YOverlapBoundary_Tooltip, 0f, 1f, 0.05f)
            .AddNumberOption(config => config.RandomColorMin, I18n.Config_RandomColorMin_Name, null, 0, 255)
            .AddNumberOption(config => config.RandomColorMax, I18n.Config_RandomColorMax_Name, null, 0, 255)
            // 肖像外观
            .AddSectionTitle(I18n.Config_PortraitAppearanceTitle_Name)
            .AddNumberOption(config => config.PortraitScale, I18n.Config_PortraitScale_Name)
            .AddNumberOption(config => config.PortraitTintR, I18n.Config_PortraitTintR_Name, null, 0, 255)
            .AddNumberOption(config => config.PortraitTintG, I18n.Config_PortraitTintG_Name, null, 0, 255)
            .AddNumberOption(config => config.PortraitTintB, I18n.Config_PortraitTintB_Name, null, 0, 255)
            .AddNumberOption(config => config.PortraitTintA, I18n.Config_PortraitTintA_Name, null, 0, 255);
    }

    /// <summary>把“基础任务配置”子配置渲染成一个分区（权重、奖励倍率、天数三个数值选项），原版与 RSV 八处任务子配置共用。</summary>
    /// <param name="section">任务子配置分区构建器。</param>
    private static void ConfigureBaseQuestSection(ConfigMenuSection<ModConfig, BaseQuestConfig> section)
    {
        section
            .AddNumberOption(config => config.Weight, I18n.Config_QuestWeight_Name)
            .AddNumberOption(config => config.RewardMultiplier, I18n.Config_QuestRewardMultiplier_Name, null, 0.25f, 5f, 0.25f)
            .AddNumberOption(config => config.Days, I18n.Config_QuestDays_Name, I18n.Config_QuestDays_Tooltip, 0, 10, 1);
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        this.ClearCache();

        VanillaQuestManager.Instance.InitVanillaQuestList();

        if (IsRSVLoaded && ModConfig.Instance.RSVConfig.EnableRSVQuestBoard)
        {
            RSVQuestManager.Instance.InitRSVQuestList();
        }
    }

    private void ClearCache()
    {
        QuestItemManager.Instance.ClearCache();
        QuestMonsterManager.Instance.ClearCache();
        VanillaQuestManager.Instance.ClearCache();
        RSVQuestManager.Instance.ClearCache();
        BaseQuestBoard.ClearCache();
    }
}
