﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.FreeLock.Framework;

namespace weizinai.StardewValleyMod.FreeLock;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonChanged;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady || !Game1.viewportFreeze) return;

        PanScreenHelper.PanScreenByMouse(this.config.MoveSpeed, this.config.MoveThreshold);
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        if (this.config.FreeLockKeybind.JustPressed())
        {
            Game1.viewportFreeze = !Game1.viewportFreeze;
            var message = new HUDMessage(Game1.viewportFreeze ? I18n.UI_ViewportUnlocked() : I18n.UI_ViewportLocked())
            {
                noIcon = true
            };
            Game1.addHUDMessage(message);
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForFreeLock(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}