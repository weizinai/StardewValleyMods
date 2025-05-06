using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.MiniMineShaft.Framework;
using weizinai.StardewValleyMod.MiniMineShaft.Patcher;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.MiniMineShaft;

internal class ModEntry : Mod
{
    private readonly KeybindList testKey1 = new(SButton.O);
    private readonly KeybindList testKey2 = new(SButton.U);

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Logger.Init(this.Monitor);

        // 注册事件
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;

        helper.Events.Player.Warped += this.OnWarped;

        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

        helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;

        // 注册Harmony补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID, new Game1Patcher());
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        MiniMine.ClearActiveMines();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        MiniMine.UpdateMines(Game1.currentGameTime);
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        MiniMine.UpdateMines10Minutes();
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.OldLocation is MiniMine)
        {
            if (Game1.IsServer)
            {
                MiniMine.ClearInactiveMines();
            }
            else
            {
                this.Helper.Multiplayer.SendMessage(
                    "",
                    "RefreshMine",
                    new[] { "weizinai.MiniMine" },
                    new[] { Game1.MasterPlayer.UniqueMultiplayerID }
                );
            }
        }
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (this.testKey1.JustPressed())
        {
            if (Game1.IsClient)
            {
                this.Helper.Multiplayer.SendMessage(
                    "",
                    "MiniMine",
                    new[] { "weizinai.MiniMine" },
                    new[] { Game1.MasterPlayer.UniqueMultiplayerID }
                );
            }
            MiniMine.EnterMine(MiniMine.GetMine(MiniMine.GetMineName(Game1.player.UniqueMultiplayerID)));
        }

        if (this.testKey2.JustPressed())
        {
            foreach (var activeMineShaft in MiniMine.ActiveMineShafts)
            {
                Logger.Info(activeMineShaft.Name);
            }

            // MiniMine.ClearActiveMines();
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        switch (e.Type)
        {
            case "MiniMine":
                MiniMine.GetMine(MiniMine.GetMineName(e.FromPlayerID));
                break;
            case "RefreshMine":
                MiniMine.ClearInactiveMines();
                break;
        }
    }
}