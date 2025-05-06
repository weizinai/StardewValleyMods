using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;
using weizinai.StardewValleyMod.MultiplayerModLimit.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.MultiplayerModLimit;

internal class ModEntry : Mod
{
    private GenericModConfigMenuIntegration<ModConfig>? configMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        Broadcaster.Init(this);
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
        Logger.Info(I18n.UI_GenerateModList());
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
            Logger.Info(I18n.UI_AddMod_Exist());
            return;
        }
        targetModList.Add(id);
        this.Helper.WriteConfig(ModConfig.Instance);
        Logger.Info(I18n.UI_AddMod_Success());
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
            Logger.Info(I18n.UI_DelMod_Fail());
            return;
        }
        targetModList.Remove(id);
        this.Helper.WriteConfig(ModConfig.Instance);
        Logger.Info(I18n.UI_DelMod_Success());
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

        Logger.Alert(I18n.UI_ListMod_Tooltip());
        foreach (var id in targetModList) Logger.Info(id);
    }

    /// <summary>
    /// 获取主机玩家安装的所有模组
    /// </summary>
    private List<string> GetAllMods()
    {
        return this.Helper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID).ToList();
    }

    private void LoadConfigMenu()
    {
        this.configMenu = this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForMultiplayerModLimit(),
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
    }

    private void ReloadConfigMenu()
    {
        this.configMenu?.Unregister();
        this.LoadConfigMenu();
    }
}