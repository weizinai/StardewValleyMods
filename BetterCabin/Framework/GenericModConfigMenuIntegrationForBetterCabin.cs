using weizinai.StardewValleyMod.Common.Integrations;
using StardewModdingAPI;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;

namespace weizinai.StardewValleyMod.BetterCabin.Framework;

internal class GenericModConfigMenuIntegrationForBetterCabin
{
    private readonly GenericModConfigMenuIntegration<ModConfig> configMenu;

    public GenericModConfigMenuIntegrationForBetterCabin(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action save, Action reset)
    {
        configMenu = new GenericModConfigMenuIntegration<ModConfig>(helper.ModRegistry, manifest, getConfig, save, reset);
    }

    public void Register()
    {
        if (!configMenu.IsLoaded) return;

        configMenu
            .Register()
            // 拜访小屋信息
            .AddSectionTitle(I18n.Config_VisitCabinInfo_Name)
            .AddBoolOption(
                config => config.VisitCabinInfo,
                (config, value) => config.VisitCabinInfo = value,
                I18n.Config_VisitCabinInfo_Name,
                I18n.Config_VisitCabinInfo_Tooltip
            )
            // 小屋主人名字标签
            .AddSectionTitle(I18n.Config_CabinOwnerNameTag_Name)
            .AddBoolOption(
                config => config.CabinOwnerNameTag,
                (config, value) => config.CabinOwnerNameTag = value,
                I18n.Config_CabinOwnerNameTag_Name,
                I18n.Config_CabinOwnerNameTag_Tooltip
            )
            .AddNumberOption(
                config => config.NameTagXOffset,
                (config, value) => config.NameTagXOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.NameTagYOffset,
                (config, value) => config.NameTagYOffset = value,
                I18n.Config_YOffset_Name
            )
            // 总在线时间标签
            .AddSectionTitle(I18n.Config_TotalOnlineTimeTag_Name)
            .AddBoolOption(
                config => config.TotalOnlineTime.Enable,
                (config, value) => config.TotalOnlineTime.Enable = value,
                I18n.Config_TotalOnlineTimeTag_Name,
                I18n.Config_TotalOnlineTimeTag_Tooltip
            )
            .AddNumberOption(
                config => config.TotalOnlineTime.XOffset,
                (config, value) => config.TotalOnlineTime.XOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.TotalOnlineTime.YOffset,
                (config, value) => config.TotalOnlineTime.YOffset = value,
                I18n.Config_YOffset_Name
            )
            // 上次在线时间标签
            .AddSectionTitle(I18n.Config_LastOnlineTimeTag_Name)
            .AddBoolOption(
                config => config.LastOnlineTime.Enable,
                (config, value) => config.LastOnlineTime.Enable = value,
                I18n.Config_LastOnlineTimeTag_Name,
                I18n.Config_LastOnlineTimeTag_Tooltip
            )
            .AddNumberOption(
                config => config.LastOnlineTime.XOffset,
                (config, value) => config.LastOnlineTime.XOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.LastOnlineTime.YOffset,
                (config, value) => config.LastOnlineTime.YOffset = value,
                I18n.Config_YOffset_Name
            )
            // 删除小屋主人
            .AddSectionTitle(I18n.Config_DeleteFarmhand_Name)
            .AddBoolOption(
                config => config.DeleteFarmhand,
                (config, value) => config.DeleteFarmhand = value,
                I18n.Config_DeleteFarmhand_Name,
                I18n.Config_DeleteFarmhand_Tooltip
            )
            .AddKeybindList(
                config => config.DeleteFarmhandKeybind,
                (config, value) => config.DeleteFarmhandKeybind = value,
                I18n.Config_DeleteFarmhandKeybind_Name
            )
            ;
    }
}