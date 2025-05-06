using System.Linq;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.HelpWanted.Framework;

internal class GenericModConfigMenuIntegrationForHelpWanted : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            .AddPageLink("VanillaConfig", I18n.Config_VanillaConfigPage_Name)
            .AddPageLink("RSVConfig", I18n.Config_RSVConfigPage_Name, enable: ModEntry.IsRSVLoaded)
            .AddPageLink("Appearance", I18n.Config_AppearancePage_Name);

        this.AddVanillaConfigPage(configMenu);
        this.AddRSVConfigPage(configMenu);
        this.AddAppearancePage(configMenu);
    }

    private void AddVanillaConfigPage(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
            // 一般设置
            .AddPage("VanillaConfig", I18n.Config_VanillaConfigPage_Name)
            .AddSectionTitle(I18n.Config_GeneralSettingsTitle_Name)
            .AddBoolOption(
                config => config.VanillaConfig.QuestFirstDay,
                (config, value) => config.VanillaConfig.QuestFirstDay = value,
                I18n.Config_QuestFirstDay_Name,
                I18n.Config_QuestFirstDay_Tooltip
            )
            .AddBoolOption(
                config => config.VanillaConfig.QuestFestival,
                (config, value) => config.VanillaConfig.QuestFestival = value,
                I18n.Config_QuestFestival_Name,
                I18n.Config_QuestFestival_Tooltip
            )
            .AddNumberOption(
                config => config.VanillaConfig.DailyQuestChance,
                (config, value) => config.VanillaConfig.DailyQuestChance = value,
                I18n.Config_DailyQuestChance_Name,
                null,
                0f,
                1f,
                0.05f
            )
            .AddBoolOption(
                config => config.VanillaConfig.OneQuestPerVillager,
                (config, value) => config.VanillaConfig.OneQuestPerVillager = value,
                I18n.Config_OneQuestPerVillager_Name
            )
            .AddBoolOption(
                config => config.VanillaConfig.ExcludeMaxHeartsNPC,
                (config, value) => config.VanillaConfig.ExcludeMaxHeartsNPC = value,
                I18n.Config_ExcludeMaxHeartsNPC_Name
            )
            .AddTextOption(
                config => string.Join(", ", config.VanillaConfig.ExcludeNPCList),
                (config, value) => config.VanillaConfig.ExcludeNPCList = value.Split(',').Select(s => s.Trim()).ToList(),
                I18n.Config_ExcludeNPCList_Name,
                I18n.Config_ExcludeNPCList_Tooltip
            )
            .AddNumberOption(
                config => config.VanillaConfig.MaxQuests,
                (config, value) => config.VanillaConfig.MaxQuests = value,
                I18n.Config_MaxQuests_Name
            )
            // 交易任务
            .AddBaseQuestConfig(
                config => config.VanillaConfig.ItemDeliveryQuestConfig,
                I18n.Config_ItemDeliveryQuestTitle_Name
            )
            .AddNumberOption(
                config => config.VanillaConfig.ItemDeliveryFriendshipGain,
                (config, value) => config.VanillaConfig.ItemDeliveryFriendshipGain = value,
                I18n.Config_ItemDeliveryFriendshipGain_Name
            )
            .AddBoolOption(
                config => config.VanillaConfig.RewriteQuestItem,
                (config, value) => config.VanillaConfig.RewriteQuestItem = value,
                I18n.Config_RewriteQuestItem_Name,
                I18n.Config_RewriteQuestItem_Tooltip
            )
            .AddNumberOption(
                config => config.VanillaConfig.QuestItemRequirement,
                (config, value) => config.VanillaConfig.QuestItemRequirement = value,
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
            .AddBoolOption(
                config => config.VanillaConfig.AllowArtisanGoods,
                (config, value) => config.VanillaConfig.AllowArtisanGoods = value,
                I18n.Config_AllowArtisanGoods_Name,
                I18n.Config_AllowArtisanGoods_Tooltip
            )
            .AddNumberOption(
                config => config.VanillaConfig.MaxPrice,
                (config, value) => config.VanillaConfig.MaxPrice = value,
                I18n.Config_MaxPrice_Name,
                I18n.Config_MaxPrice_Tooltip
            )

            // 采集任务
            .AddBaseQuestConfig(
                config => config.VanillaConfig.ResourceCollectionQuestConfig,
                I18n.Config_ResourceCollectionQuestTitle_Name
            )
            .AddBoolOption(
                config => config.VanillaConfig.MoreResourceCollectionQuest,
                (config, value) => config.VanillaConfig.MoreResourceCollectionQuest = value,
                I18n.Config_MoreResourceCollectionQuest_Name
            )

            // 钓鱼任务
            .AddBaseQuestConfig(
                config => config.VanillaConfig.FishingQuestConfig,
                I18n.Config_FishingQuestTitle_Name
            )

            // 杀怪任务
            .AddBaseQuestConfig(
                config => config.VanillaConfig.SlayMonsterQuestConfig,
                I18n.Config_SlayMonstersQuestTitle_Name
            )
            .AddBoolOption(
                config => config.VanillaConfig.MoreSlayMonsterQuest,
                (config, value) => config.VanillaConfig.MoreSlayMonsterQuest = value,
                I18n.Config_MoreSlayMonsterQuests_Name
            );
    }

    private void AddRSVConfigPage(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
            .AddPage("RSVConfig", I18n.Config_RSVConfigPage_Name)
            // 一般设置
            .AddSectionTitle(I18n.Config_GeneralSettingsTitle_Name)
            .AddBoolOption(
                config => config.RSVConfig.EnableRSVQuestBoard,
                (config, value) => config.RSVConfig.EnableRSVQuestBoard = value,
                I18n.Config_EnableRSVQuestBoard_Name
            )
            .AddBoolOption(
                config => config.RSVConfig.QuestFirstDay,
                (config, value) => config.RSVConfig.QuestFirstDay = value,
                I18n.Config_QuestFirstDay_Name,
                I18n.Config_QuestFirstDay_Tooltip
            )
            .AddBoolOption(
                config => config.RSVConfig.QuestFestival,
                (config, value) => config.RSVConfig.QuestFestival = value,
                I18n.Config_QuestFestival_Name,
                I18n.Config_QuestFestival_Tooltip
            )
            .AddNumberOption(
                config => config.RSVConfig.DailyQuestChance,
                (config, value) => config.RSVConfig.DailyQuestChance = value,
                I18n.Config_DailyQuestChance_Name,
                null,
                0f,
                1f,
                0.05f
            )
            .AddNumberOption(
                config => config.RSVConfig.MaxQuests,
                (config, value) => config.RSVConfig.MaxQuests = value,
                I18n.Config_MaxQuests_Name
            )
            .AddBoolOption(
                config => config.RSVConfig.AllowSameQuest,
                (config, value) => config.RSVConfig.AllowSameQuest = value,
                I18n.Config_AllowSameQuest_Name
            )

            // 交易任务
            .AddBaseQuestConfig(
                config => config.RSVConfig.ItemDeliveryQuestConfig,
                I18n.Config_ItemDeliveryQuestTitle_Name
            )

            // 钓鱼任务
            .AddBaseQuestConfig(
                config => config.RSVConfig.FishingQuestConfig,
                I18n.Config_FishingQuestTitle_Name
            )

            // 杀怪任务
            .AddBaseQuestConfig(
                config => config.RSVConfig.SlayMonsterQuestConfig,
                I18n.Config_SlayMonstersQuestTitle_Name
            )

            // 丢失物品任务
            .AddBaseQuestConfig(
                config => config.RSVConfig.LostItemQuestConfig,
                I18n.Config_LostItemQuestTitle_Name
            );
    }

    private void AddAppearancePage(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        // 外观
        configMenu
            .AddPage("Appearance", I18n.Config_AppearancePage_Name)
            .AddSectionTitle(I18n.Config_NoteAppearanceTitle_Name)
            .AddNumberOption(
                config => config.NoteScale,
                (config, value) => config.NoteScale = value,
                I18n.Config_NoteScale_Name
            )
            .AddNumberOption(
                config => config.XOverlapBoundary,
                (config, value) => config.XOverlapBoundary = value,
                I18n.Config_XOverlapBoundary_Name,
                I18n.Config_XOverlapBoundary_Tooltip,
                0,
                1,
                0.05f
            )
            .AddNumberOption(
                config => config.YOverlapBoundary,
                (config, value) => config.YOverlapBoundary = value,
                I18n.Config_YOverlapBoundary_Name,
                I18n.Config_YOverlapBoundary_Tooltip,
                0,
                1,
                0.05f
            )
            .AddNumberOption(
                config => config.RandomColorMin,
                (config, value) => config.RandomColorMin = value,
                I18n.Config_RandomColorMin_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.RandomColorMax,
                (config, value) => config.RandomColorMax = value,
                I18n.Config_RandomColorMax_Name,
                null,
                0,
                255
            )
            .AddSectionTitle(I18n.Config_PortraitAppearanceTitle_Name)
            .AddNumberOption(
                config => config.PortraitScale,
                (config, value) => config.PortraitScale = value,
                I18n.Config_PortraitScale_Name
            )
            .AddNumberOption(
                config => config.PortraitOffsetX,
                (config, value) => config.PortraitOffsetX = value,
                I18n.Config_PortraitOffsetX_Name
            )
            .AddNumberOption(
                config => config.PortraitOffsetY,
                (config, value) => config.PortraitOffsetY = value,
                I18n.Config_PortraitOffsetY_Name
            )
            .AddNumberOption(
                config => config.PortraitTintR,
                (config, value) => config.PortraitTintR = value,
                I18n.Config_PortraitTintR_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintG,
                (config, value) => config.PortraitTintG = value,
                I18n.Config_PortraitTintG_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintB,
                (config, value) => config.PortraitTintB = value,
                I18n.Config_PortraitTintB_Name,
                null,
                0,
                255
            )
            .AddNumberOption(
                config => config.PortraitTintA,
                (config, value) => config.PortraitTintA = value,
                I18n.Config_PortraitTintA_Name,
                null,
                0,
                255
            );
    }
}