using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;
using weizinai.StardewValleyMod.MultiplayerModLimit.Handler;
using weizinai.StardewValleyMod.PiCore.Extension;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.MultiplayerModLimit;

internal class ModEntry : Mod
{
    private GenericModConfigMenuIntegration<ModConfig>? configMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        Broadcaster<ModEntry>.Init(this);
        ModConfig.Init(helper);

        new KickPlayerHandler(helper).Apply();

        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        // 注册命令
        helper.ConsoleCommands.Add("generate_allow", "", this.GenerateModListCommand);
        helper.ConsoleCommands.Add("generate_require", "", this.GenerateModListCommand);
        helper.ConsoleCommands.Add("generate_ban", "", this.GenerateModListCommand);
        helper.ConsoleCommands.Add("add_allow", "", this.AddModToCurrentListCommand);
        helper.ConsoleCommands.Add("add_require", "", this.AddModToCurrentListCommand);
        helper.ConsoleCommands.Add("add_ban", "", this.AddModToCurrentListCommand);
        helper.ConsoleCommands.Add("del_allow", "", this.DelModInCurrentListCommand);
        helper.ConsoleCommands.Add("del_require", "", this.DelModInCurrentListCommand);
        helper.ConsoleCommands.Add("del_ban", "", this.DelModInCurrentListCommand);
        helper.ConsoleCommands.Add("list_allow", "", this.ListCurrentListCommand);
        helper.ConsoleCommands.Add("list_require", "", this.ListCurrentListCommand);
        helper.ConsoleCommands.Add("list_ban", "", this.ListCurrentListCommand);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.LoadConfigMenu();
    }

    private void GenerateModListCommand(string command, string[] args)
    {
        var targetModList = command switch
        {
            "generate_allow" => ModConfig.Instance.AllowedModList,
            "generate_require" => ModConfig.Instance.RequiredModList,
            "generate_ban" => ModConfig.Instance.BannedModList,
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };
        targetModList[args[0]] = this.GetAllMods();
        this.Helper.WriteConfig(ModConfig.Instance);
        this.ReloadConfigMenu();
        Logger<ModEntry>.Info(I18n.UI_GenerateModList());
    }

    private void AddModToCurrentListCommand(string command, string[] args)
    {
        var targetModList = command switch
        {
            "add_allow" => ModConfig.Instance.AllowedModList[ModConfig.Instance.AllowedModListSelected],
            "add_require" => ModConfig.Instance.RequiredModList[ModConfig.Instance.RequiredModListSelected],
            "add_ban" => ModConfig.Instance.BannedModList[ModConfig.Instance.BannedModListSelected],
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };

        var id = args[0];

        if (targetModList.Contains(id))
        {
            Logger<ModEntry>.Info(I18n.UI_AddMod_Exist());

            return;
        }

        targetModList.Add(id);
        this.Helper.WriteConfig(ModConfig.Instance);
        Logger<ModEntry>.Info(I18n.UI_AddMod_Success());
    }

    private void DelModInCurrentListCommand(string command, string[] args)
    {
        var targetModList = command switch
        {
            "del_allow" => ModConfig.Instance.AllowedModList[ModConfig.Instance.AllowedModListSelected],
            "del_require" => ModConfig.Instance.RequiredModList[ModConfig.Instance.RequiredModListSelected],
            "del_ban" => ModConfig.Instance.BannedModList[ModConfig.Instance.BannedModListSelected],
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };

        var id = args[0];

        if (!targetModList.Contains(id))
        {
            Logger<ModEntry>.Info(I18n.UI_DelMod_Fail());

            return;
        }

        targetModList.Remove(id);
        this.Helper.WriteConfig(ModConfig.Instance);
        Logger<ModEntry>.Info(I18n.UI_DelMod_Success());
    }

    private void ListCurrentListCommand(string command, string[] args)
    {
        var targetModList = command switch
        {
            "list_allow" => ModConfig.Instance.AllowedModList[ModConfig.Instance.AllowedModListSelected],
            "list_require" => ModConfig.Instance.RequiredModList[ModConfig.Instance.RequiredModListSelected],
            "list_ban" => ModConfig.Instance.BannedModList[ModConfig.Instance.BannedModListSelected],
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };

        Logger<ModEntry>.Alert(I18n.UI_ListMod_Tooltip());

        foreach (var id in targetModList)
        {
            Logger<ModEntry>.Info(id);
        }
    }

    /// <summary>
    ///     获取主机玩家安装的所有模组
    /// </summary>
    private List<string> GetAllMods()
    {
        return this.Helper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID).ToList();
    }

    private void LoadConfigMenu()
    {
        this.configMenu = this.AddGenericModConfigMenu(
            () => ModConfig.Instance,
            value => ModConfig.Instance = value,
            this.AddConfigMenu
        );
    }

    private void AddConfigMenu(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
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
                I18n.Config_KickPlayerDelayTime_Tooltip
            )
            // 发送SMAPI信息
            .AddBoolOption(
                config => config.SendSMAPIInfo,
                (config, value) => config.SendSMAPIInfo = value,
                I18n.Config_SendSMAPIInfo_Name,
                I18n.Config_SendSMAPIInfo_Tooltip
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

    private void ReloadConfigMenu()
    {
        this.configMenu?.Unregister();
        this.LoadConfigMenu();
    }
}
