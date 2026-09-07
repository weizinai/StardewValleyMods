using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;
using weizinai.StardewValleyMod.MultiplayerModLimit.Handler;
using weizinai.StardewValleyMod.PiCore.Config;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Logging;

namespace weizinai.StardewValleyMod.MultiplayerModLimit;

internal class ModEntry : Mod
{
    private ConfigService<ModConfig> configService = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger<ModEntry>.Init(this);
        Broadcaster<ModEntry>.Init(this);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置；控制台命令写配置后经同一服务保存并重载菜单，故保留字段引用
        this.configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        this.configService.RegisterMenu(this.BuildConfigMenu);

        new KickPlayerHandler(helper).Apply();

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
        // 经配置模块保存配置并重载菜单，让新增的列表键出现在下拉允许值中
        this.configService.Save();
        this.configService.ReloadMenu();
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
        this.configService.Save();
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
        this.configService.Save();
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
    /// 获取主机玩家安装的所有模组
    /// </summary>
    private List<string> GetAllMods()
    {
        return this.Helper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID).ToList();
    }

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu
            // 一般设置
            .AddSectionTitle(I18n.Config_GeneralSettingTitle_Name)
            .AddBoolOption(config => config.EnableMod, I18n.Config_EnableMod_Name)
            .AddBoolOption(
                config => config.ShowMismatchedModInfo,
                I18n.Config_ShowMismatchedModInfo_Name,
                I18n.Config_ShowMismatchedModInfo_Tooltip
            )
            .AddBoolOption(config => config.KickPlayer, I18n.Config_KickPlayer_Name, I18n.Config_KickPlayer_Tooltip)
            .AddNumberOption(config => config.KickPlayerDelayTime, I18n.Config_KickPlayerDelayTime_Name, I18n.Config_KickPlayerDelayTime_Tooltip)
            .AddBoolOption(config => config.SendSMAPIInfo, I18n.Config_SendSMAPIInfo_Name, I18n.Config_SendSMAPIInfo_Tooltip)
            // 限制设置
            .AddSectionTitle(I18n.Config_LimitSettingTitle_Name)
            .AddBoolOption(config => config.RequireSMAPI, I18n.Config_RequireSMAPI_Name, I18n.Config_RequireSMAPI_Tooltip)
            .AddEnumOption(
                config => config.LimitMode,
                I18n.Config_LimitMode_Name,
                I18n.Config_LimitMode_Tooltip,
                formatValue: value => value switch
                {
                    LimitMode.WhiteListMode => I18n.Config_LimitMode_WhiteListMode(),
                    LimitMode.BlackListMode => I18n.Config_LimitMode_BlackListMode(),
                    _ => value.ToString()
                }
            )
            // 选择的模组列表设置
            .AddSectionTitle(I18n.Config_ModListSelectedTitle_Name)
            .AddParagraph(I18n.Config_ModListSelected_Paragraph)
            // 逃生舱：三个“选择的模组列表”下拉的允许值来自各模组列表字典的键，控制台命令新增列表键后经 ReloadMenu 重跑本段以刷新
            .AddCustomSection(this.AddModListSelectedOptions);
    }

    /// <summary>逃生舱：注册三个“选择的模组列表”下拉，其允许值为对应列表字典当前的键集合。</summary>
    /// <param name="configMenu">原始 GMCM 集成对象。</param>
    private void AddModListSelectedOptions(GenericModConfigMenuIntegration<ModConfig> configMenu)
    {
        configMenu
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
