using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network.NetReady;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.ReadyCheckKick.Framework;

namespace weizinai.StardewValleyMod.ReadyCheckKick.Handler;

internal class ReadyCheckDialogueHandler : BaseHandler
{
    private readonly MethodInfo getIfExistsMethod;
    private readonly FieldInfo readyStatesField;

    private bool isAutoKickUnreadyFarmers;
    private readonly Dictionary<long, string> unreadyFarmers = new();

    public ReadyCheckDialogueHandler(IModHelper helper) : base(helper)
    {
        this.getIfExistsMethod = typeof(ReadySynchronizer).GetMethod("GetIfExists", BindingFlags.Instance | BindingFlags.NonPublic)!;
        this.readyStatesField = Assembly
            .LoadFrom("Stardew Valley.dll")
            .GetType("StardewValley.Network.NetReady.Internal.ServerReadyCheck")
            !.GetField("ReadyStates", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public override void Apply()
    {
        this.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        this.Helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;

        this.Helper.ConsoleCommands.Add("kup", "", this.KickUnreadyFarmersCommand);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        this.UpdateUnreadyFarmers();
        this.AutoKickUnreadyFarmers();
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
    {
        if (!ModConfig.Instance.ShowInfoInReadyCheckDialogue) return;

        if (!this.IsServerReady(out _)) return;

        if (!this.unreadyFarmers.Any()) return;

        // 文字绘制逻辑
        var text = string.Join("\n", this.unreadyFarmers.Values);
        var position = new Vector2(64, 64);
        e.SpriteBatch.DrawString(Game1.dialogueFont, text, position, Color.Red);
    }

    private void UpdateUnreadyFarmers()
    {
        if (!this.IsServerReady(out var menu)) return;

        var serverReadyCheck = this.getIfExistsMethod.Invoke(Game1.netReady, new object?[] { menu.checkName });
        var rawReadyStates = (IDictionary)this.readyStatesField.GetValue(serverReadyCheck)!;

        this.unreadyFarmers.Clear();
        foreach (DictionaryEntry entry in rawReadyStates)
        {
            var id = (long)entry.Key;
            if (entry.Value!.ToString() == "NotReady")
            {
                var farmer = Game1.GetPlayer(id);
                if (farmer is not null)
                    this.unreadyFarmers.Add(id, farmer.displayName);
                else
                    Logger.Error($"Players with {id} id could not be found");
            }
        }
    }

    private void AutoKickUnreadyFarmers()
    {
        var config = ModConfig.Instance;

        if (!config.AutoKickUnreadyFarmers) return;

        if (!this.IsServerReady(out var menu)) return;

        switch (menu.checkName)
        {
            case "festivalStart" when ModConfig.Instance.SpecialTreatForFestival:
            {
                var festivalId = $"{Game1.currentSeason}{Game1.dayOfMonth}";
                if (Event.tryToLoadFestivalData(festivalId, out _, out _, out _, out _, out var endTime))
                {
                    if (Game1.timeOfDay == endTime - 50)
                    {
                        Logger.Info(I18n.UI_AutoKickUnreadyFarmers_FestivalTooltip());
                        this.KickUnreadyFarmers();
                    }
                }
                break;
            }
            default:
            {
                var readyPlayerRatio = (float)Game1.netReady.GetNumberReady(menu.checkName)
                                       / Game1.netReady.GetNumberRequired(menu.checkName);

                if (readyPlayerRatio > config.AutoKickUnreadyFarmersRatio && !this.isAutoKickUnreadyFarmers)
                {
                    this.isAutoKickUnreadyFarmers = true;
                    Logger.Info(I18n.UI_AutoKickUnreadyFarmers_DefaultTooltip(config.AutoKickUnreadyFarmersRatio, config.AutoKickUnreadyFarmersDelay));
                    DelayedAction.functionAfterDelay(() =>
                    {
                        if (Game1.activeClickableMenu is ReadyCheckDialog)
                        {
                            this.KickUnreadyFarmers();
                        }
                        this.isAutoKickUnreadyFarmers = false;
                    }, config.AutoKickUnreadyFarmersDelay * 1000);
                }
                break;
            }
        }
    }

    private void KickUnreadyFarmersCommand(string command, string[] args)
    {
        if (!this.IsServerReady(out _))
        {
            Logger.Error("This command can only be used on the host and when the current active menu is a 'ReadyCheckDialog' menu");
            return;
        }

        this.KickUnreadyFarmers();
    }

    private void KickUnreadyFarmers()
    {
        foreach (var (id, name) in this.unreadyFarmers)
        {
            Logger.Info(I18n.UI_KickUnreadyFarmer_Tooltip(name));
            Game1.server.kick(id);
        }
        this.unreadyFarmers.Clear();
    }

    private bool IsServerReady([NotNullWhen(true)] out ReadyCheckDialog? menu)
    {
        if (Game1.IsServer && Game1.activeClickableMenu is ReadyCheckDialog dialog)
        {
            menu = dialog;
            return true;
        }

        menu = null;
        return false;
    }
}