using Common.Integrations;
using StardewModdingAPI;

namespace BetterCabin.Framework;

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
                config => config.XOffset,
                (config, value) => config.XOffset = value,
                I18n.Config_XOffset_Name
            )
            .AddNumberOption(
                config => config.YOffset,
                (config, value) => config.YOffset = value,
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