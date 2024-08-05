using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

namespace weizinai.StardewValleyMod.MultiplayerModLimit;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private GenericModConfigMenuIntegrationForMultiplayerModLimit configMenu = null!;
    private readonly List<PlayerSlot> playersToKick = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        MultiplayerLog.Init(this);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
        // 注册命令
        helper.ConsoleCommands.Add("generate_allow", "", this.GenerateModList);
        helper.ConsoleCommands.Add("generate_require", "", this.GenerateModList);
        helper.ConsoleCommands.Add("generate_ban", "", this.GenerateModList);
        helper.ConsoleCommands.Add("add_allow", "", this.AddModToCurrentList);
        helper.ConsoleCommands.Add("add_require", "", this.AddModToCurrentList);
        helper.ConsoleCommands.Add("add_ban", "", this.AddModToCurrentList);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.configMenu = new GenericModConfigMenuIntegrationForMultiplayerModLimit(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => this.config,
                () =>
                {
                    this.config = new ModConfig();
                    this.Helper.WriteConfig(this.config);
                },
                () => this.Helper.WriteConfig(this.config)
            )
        );

        this.configMenu.Register();
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!this.IsModEnable()) return;

        if (!this.config.KickPlayer) return;

        foreach (var player in this.playersToKick)
        {
            player.TimeLeft--;
            if (player.TimeLeft < 0)
            {
                try
                {
                    Game1.server.kick(player.Id);
                }
                catch (Exception)
                {
                    Game1.otherFarmers.Remove(player.Id);
                }

            }
        }

        this.playersToKick.RemoveAll(player => player.TimeLeft < 0);
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        this.playersToKick.Clear();
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (!this.IsModEnable()) return;

        var name = Game1.getFarmer(e.Peer.PlayerID).Name;

        if (this.config.RequireSMAPI && !e.Peer.HasSmapi)
        {
            this.KickPlayerWithoutSMAPI(name, e.Peer.PlayerID);
            return;
        }

        if (e.Peer.HasSmapi)
        {
            var unAllowedMods = this.GetUnAllowedMods(e.Peer);

            if (unAllowedMods["Required"].Any() || unAllowedMods["Banned"].Any())
            {
                if (this.config.KickPlayer)
                {
                    this.playersToKick.Add(new PlayerSlot(e.Peer.PlayerID, this.config.KickPlayerDelayTime));
                }
                this.ShowMismatchedModInfo(unAllowedMods, name);
                this.SendModRequirementInfo(unAllowedMods, e.Peer.PlayerID);
                Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, I18n.UI_KickPlayer_ClientTooltip(), e.Peer.PlayerID);
                Game1.chatBox.addInfoMessage(I18n.UI_KickPlayer_ServerTooltip(name));
            }
        }
    }

    private void GenerateModList(string command, string[] args)
    {
        var targetModList = command switch
        {
            "generate_allow" => this.config.AllowedModList,
            "generate_require" => this.config.RequiredModList,
            "generate_ban" => this.config.BannedModList,
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };
        targetModList[args[0]] = this.GetAllMods();
        this.Helper.WriteConfig(this.config);
        this.configMenu.Reset();
        Log.Info(I18n.UI_GenerateModList());
    }

    private void AddModToCurrentList(string command, string[] args)
    {
        var targetModList = command switch
        {
            "add_allow" => this.config.AllowedModList[this.config.AllowedModListSelected],
            "add_require" => this.config.RequiredModList[this.config.RequiredModListSelected],
            "add_ban" => this.config.BannedModList[this.config.BannedModListSelected],
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };

        var id = args[0];
        if (targetModList.Contains(id))
        {
            Log.Info(I18n.UI_AddMod_Exist());
            return;
        }
        targetModList.Add(id);
        this.Helper.WriteConfig(this.config);
        Log.Info(I18n.UI_AddMod_Success());
    }

    /// <summary>
    /// 踢出未安装SMAPI的玩家
    /// </summary>
    private void KickPlayerWithoutSMAPI(string playerName, long playerId)
    {
        if (this.config.KickPlayer)
        {
            this.playersToKick.Add(new PlayerSlot(playerId, this.config.KickPlayerDelayTime));
        }
        Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, I18n.UI_RequireSMAPI_ClientTooltip(), playerId);
        Game1.chatBox.addInfoMessage(I18n.UI_RequireSMAPI_ServerTooltip(playerName));
    }

    /// <summary>
    /// 获取某个玩家不满足要求的模组数据
    /// </summary>
    private Dictionary<string, List<string>> GetUnAllowedMods(IMultiplayerPeer peer)
    {
        var detectedMods = peer.Mods.Select(x => x.ID).ToList();

        var unAllowedMods = new Dictionary<string, List<string>>
        {
            { "Required", new List<string>() },
            { "Banned", new List<string>() },
        };

        var allowedModList = this.config.AllowedModList[this.config.AllowedModListSelected];
        var requiredModList = this.config.RequiredModList[this.config.RequiredModListSelected];
        var bannedModList = this.config.BannedModList[this.config.BannedModListSelected];

        foreach (var id in requiredModList.Where(id => !detectedMods.Contains(id))) unAllowedMods["Required"].Add(id);

        switch (this.config.LimitMode)
        {
            case LimitMode.WhiteListMode:
            {
                foreach (var id in detectedMods.Where(id => !allowedModList.Contains(id) && !requiredModList.Contains(id)))
                {
                    unAllowedMods["Banned"].Add(id);
                }
                break;
            }
            case LimitMode.BlackListMode:
            {
                foreach (var id in detectedMods.Where(id => bannedModList.Contains(id)))
                {
                    unAllowedMods["Banned"].Add(id);
                }
                break;
            }
        }

        return unAllowedMods;
    }

    /// <summary>
    /// 在主机处的SMAPI控制台显示不匹配的模组信息
    /// </summary>
    private void ShowMismatchedModInfo(Dictionary<string, List<string>> unAllowedMods, string name)
    {
        if (!this.config.ShowMismatchedModInfo) return;

        Log.Alert(I18n.UI_KickPlayer_ServerTooltip(name));
        foreach (var id in unAllowedMods["Required"])
        {
            Log.Info(I18n.UI_ModLimit_Required(id));
        }
        Log.Info("----------");
        foreach (var id in unAllowedMods["Banned"])
        {
            Log.Info(I18n.UI_ModLimit_Banned(id));
        }
    }

    /// <summary>
    /// 像不满足模组要求的玩家的SMAPI控制太发送不满足的模组的信息
    /// </summary>
    private void SendModRequirementInfo(Dictionary<string, List<string>> unAllowedMods, long playerId)
    {
        if (!this.config.SendSMAPIInfo) return;

        var target = new[] { playerId };
        MultiplayerLog.Alert(I18n.UI_KickPlayer_ClientTooltip(), target);
        foreach (var id in unAllowedMods["Required"])
        {
            MultiplayerLog.Info(I18n.UI_ModLimit_Required(id), target);
        }
        foreach (var id in unAllowedMods["Banned"])
        {
            MultiplayerLog.Info(I18n.UI_ModLimit_Banned(id), target);
        }
    }

    /// <summary>
    /// 获取玩家安装的所有模组
    /// </summary>
    private List<string> GetAllMods()
    {
        return this.Helper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID).ToList();
    }

    private bool IsModEnable()
    {
        return Game1.IsServer && this.config.EnableMod;
    }
}