using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.MultiplayerModLimit.Framework;
using weizinai.StardewValleyMod.PiCore.Handler;

namespace weizinai.StardewValleyMod.MultiplayerModLimit.Handler;

internal class KickPlayerHandler : BaseHandler
{
    private bool IsModEnable => Context.HasRemotePlayers && Context.IsMainPlayer && ModConfig.Instance.EnableMod;

    private readonly List<PlayerSlot> playersToKick = new();

    public KickPlayerHandler(IModHelper helper) : base(helper) { }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;

        this.Helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
    }

    private void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!this.IsModEnable) return;

        if (!ModConfig.Instance.KickPlayer) return;

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
        if (!this.IsModEnable) return;

        var name = Game1.GetPlayer(e.Peer.PlayerID)?.Name ?? "";

        if (ModConfig.Instance.RequireSMAPI && !e.Peer.HasSmapi)
        {
            this.KickPlayerWithoutSMAPI(name, e.Peer.PlayerID);
            return;
        }

        if (e.Peer.HasSmapi)
        {
            var unAllowedMods = this.GetUnAllowedMods(e.Peer);

            if (unAllowedMods["Required"].Any() || unAllowedMods["Banned"].Any())
            {
                if (ModConfig.Instance.KickPlayer)
                {
                    this.playersToKick.Add(new PlayerSlot(e.Peer.PlayerID, ModConfig.Instance.KickPlayerDelayTime));
                }
                this.ShowMismatchedModInfo(unAllowedMods, name);
                this.SendModRequirementInfo(unAllowedMods, e.Peer.PlayerID);
                Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, I18n.UI_KickPlayer_ClientTooltip(), e.Peer.PlayerID);
                Game1.chatBox.addInfoMessage(I18n.UI_KickPlayer_ServerTooltip(name));
            }
        }
    }

    /// <summary>
    /// 踢出未安装SMAPI的客机玩家
    /// </summary>
    private void KickPlayerWithoutSMAPI(string playerName, long playerId)
    {
        if (ModConfig.Instance.KickPlayer)
        {
            this.playersToKick.Add(new PlayerSlot(playerId, ModConfig.Instance.KickPlayerDelayTime));
        }
        Game1.Multiplayer.sendChatMessage(LocalizedContentManager.CurrentLanguageCode, I18n.UI_RequireSMAPI_ClientTooltip(), playerId);
        Game1.chatBox.addInfoMessage(I18n.UI_RequireSMAPI_ServerTooltip(playerName));
    }

    /// <summary>
    /// 获取客机玩家不满足要求的模组
    /// </summary>
    private Dictionary<string, List<string>> GetUnAllowedMods(IMultiplayerPeer peer)
    {
        var detectedMods = peer.Mods.Select(x => x.ID).ToList();

        var unAllowedMods = new Dictionary<string, List<string>>
        {
            { "Required", new List<string>() },
            { "Banned", new List<string>() },
        };

        var allowedModList = ModConfig.Instance.AllowedModList[ModConfig.Instance.AllowedModListSelected];
        var requiredModList = ModConfig.Instance.RequiredModList[ModConfig.Instance.RequiredModListSelected];
        var bannedModList = ModConfig.Instance.BannedModList[ModConfig.Instance.BannedModListSelected];

        // 获取客机玩家没有安装的被要求的模组
        foreach (var id in requiredModList.Where(id => !detectedMods.Contains(id))) unAllowedMods["Required"].Add(id);

        // 获取客机玩家安装的被禁止的模组
        switch (ModConfig.Instance.LimitMode)
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
    /// 在主机玩家的SMAPI控制台显示客机玩家不匹配的模组信息
    /// </summary>
    private void ShowMismatchedModInfo(Dictionary<string, List<string>> unAllowedMods, string name)
    {
        if (!ModConfig.Instance.ShowMismatchedModInfo) return;

        Logger.Alert(I18n.UI_KickPlayer_ServerTooltip(name));
        foreach (var id in unAllowedMods["Required"])
        {
            Logger.Info(I18n.UI_ModLimit_Required(id));
        }
        Logger.Info("----------");
        foreach (var id in unAllowedMods["Banned"])
        {
            Logger.Info(I18n.UI_ModLimit_Banned(id));
        }
    }

    /// <summary>
    /// 向不满足模组要求的客机玩家的SMAPI控制台发送不满足的模组的信息
    /// </summary>
    private void SendModRequirementInfo(Dictionary<string, List<string>> unAllowedMods, long playerId)
    {
        if (!ModConfig.Instance.SendSMAPIInfo) return;

        var target = new[] { playerId };
        Broadcaster.Alert(I18n.UI_KickPlayer_ClientTooltip(), target);
        foreach (var id in unAllowedMods["Required"])
        {
            Broadcaster.Info(I18n.UI_ModLimit_Required(id), target);
        }
        foreach (var id in unAllowedMods["Banned"])
        {
            Broadcaster.Info(I18n.UI_ModLimit_Banned(id), target);
        }
    }
}