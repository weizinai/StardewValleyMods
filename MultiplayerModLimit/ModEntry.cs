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
        if (!this.config.EnableMod) return;

        if (!Game1.IsServer) return;

        foreach (var player in this.playersToKick)
        {
            player.Cooldown--;
            if (player.Cooldown < 0) Game1.server?.kick(player.ID);
        }

        this.playersToKick.RemoveAll(player => player.Cooldown < 0);
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        this.playersToKick.Clear();
    }

    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        // 如果玩家不是多人模式的房主，则返回
        if (!Game1.IsServer) return;

        // 如果模组未启用，则返回
        if (!this.config.EnableMod) return;

        var name = Game1.getFarmer(e.Peer.PlayerID).Name;

        if (this.config.RequireSMAPI && !e.Peer.HasSmapi)
        {
            this.playersToKick.Add(new PlayerSlot(e.Peer.PlayerID, this.config.KickPlayerDelayTime));
            Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, I18n.UI_RequireSMAPI_ClientTooltip(), e.Peer.PlayerID);
            Game1.chatBox.addInfoMessage(I18n.UI_RequireSMAPI_ServerTooltip(name));
            return;
        }

        if (e.Peer.HasSmapi)
        {
            var detectedMods = e.Peer.Mods.Select(x => x.ID).ToList();

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
                    foreach (var id in detectedMods.Where(id => !allowedModList.Contains(id) && !requiredModList.Contains(id))) unAllowedMods["Banned"].Add(id);
                    break;
                case LimitMode.BlackListMode:
                    foreach (var id in detectedMods.Where(id => bannedModList.Contains(id))) unAllowedMods["Banned"].Add(id);
                    break;
            }

            if (unAllowedMods["Required"].Any() || unAllowedMods["Banned"].Any())
            {
                this.playersToKick.Add(new PlayerSlot(e.Peer.PlayerID, this.config.KickPlayerDelayTime));
                this.SendModRequirementInfo(unAllowedMods, e.Peer.PlayerID);
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
        targetModList.Add(args[0], this.GetAllMods());
        this.Helper.WriteConfig(this.config);
        this.configMenu.Reset();
        Log.Info(I18n.UI_GenerateModList());
    }

    private List<string> GetAllMods()
    {
        return this.Helper.ModRegistry.GetAll().Select(x => x.Manifest.UniqueID).ToList();
    }

    private void SendModRequirementInfo(Dictionary<string, List<string>> unAllowedMods, long playerId)
    {
        var target = new[] { playerId };
        MultiplayerLog.Alert(I18n.UI_KickPlayer_ClientTooltip(), target);
        foreach (var id in unAllowedMods["Required"]) MultiplayerLog.Info(I18n.UI_ModLimit_Required(id), target);
        foreach (var id in unAllowedMods["Banned"]) MultiplayerLog.Info(I18n.UI_ModLimit_Banned(id), target);
    }
}