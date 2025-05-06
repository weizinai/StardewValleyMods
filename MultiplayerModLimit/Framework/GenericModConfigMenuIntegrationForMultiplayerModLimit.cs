using System;
using System.Linq;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class GenericModConfigMenuIntegrationForMultiplayerModLimit : IGenericModConfigMenuIntegrationFor<ModConfig>
{
    public void Register(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu.Register()
            .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
            // 启用模组
            .AddBoolOption(
                config => config.EnableMod,
                (config, value) => config.EnableMod = value,
                I18n.Config_EnableMod_Name
            )
            // 显示不匹配的模组信息
            .AddBoolOption(
                config => config.ShowMismatchedModInfo,
                (config, value) => config.ShowMismatchedModInfo = value,
                I18n.Config_ShowMismatchedModInfo_Name,
                I18n.Config_ShowMismatchedModInfo_Tooltip
            )
            // 踢出玩家
            .AddBoolOption(
                config => config.KickPlayer,
                (config, value) => config.KickPlayer = value,
                I18n.Config_KickPlayer_Name,
                I18n.Config_KickPlayer_Tooltip
            )
            // 踢出玩家延迟时间
            .AddNumberOption(
                config => config.KickPlayerDelayTime,
                (config, value) => config.KickPlayerDelayTime = value,
                I18n.Config_KickPlayerDelayTime_Name,
                I18n.Config_KickPlayerDelayTIme_Tooltip
            )
            // 发送SMAPI信息
            .AddBoolOption(
                config => config.SendSMAPIInfo,
                (config, value) => config.SendSMAPIInfo = value,
                I18n.Config_SendSMAPIInfo_Name,
                I18n.Config_SendSMAPIIngo_Tooltip
            )
            .AddSectionTitle(I18n.Config_LimitSettingTitle_Name)
            // 要求SMAPI
            .AddBoolOption(
                config => config.RequireSMAPI,
                (config, value) => config.RequireSMAPI = value,
                I18n.Config_RequireSMAPI_Name,
                I18n.Config_RequireSMAPI_Tooltip
            )
            // 限制模式
            .AddTextOption(
                config => config.LimitMode.ToString(),
                (config, value) => config.LimitMode = Enum.Parse<LimitMode>(value),
                I18n.Config_LimitMode_Name,
                I18n.Config_LimitMode_Tooltip,
                new[] { "WhiteListMode", "BlackListMode" },
                value => value switch
                {
                    "WhiteListMode" => I18n.Config_LimitMode_WhiteListMode(),
                    "BlackListMode" => I18n.Config_LimitMode_BlackListMode(),
                    _ => value
                }
            )
            .AddSectionTitle(I18n.Config_ModListSelectedTitle_Name)
            .AddParagraph(I18n.Config_ModListSelected_Paragraph)
            // 选择的模组列表
            .AddTextOption(
                config => config.AllowedModListSelected,
                (config, value) => config.AllowedModListSelected = value,
                I18n.Config_AllowedModListSelected_Name,
                I18n.Config_AllowedModListSelected_Tooltip,
                ModConfig.Instance.AllowedModList.Keys.ToArray()
            )
            .AddTextOption(
                config => config.RequiredModListSelected,
                (config, value) => config.RequiredModListSelected = value,
                I18n.Config_RequiredModListSelected_Name,
                I18n.Config_RequiredModListSelected_Tooltip,
                ModConfig.Instance.RequiredModList.Keys.ToArray()
            )
            .AddTextOption(
                config => config.BannedModListSelected,
                (config, value) => config.BannedModListSelected = value,
                I18n.Config_BannedModListSelected_Name,
                I18n.Config_BannedModListSelected_Tooltip,
                ModConfig.Instance.BannedModList.Keys.ToArray()
            );
    }
}