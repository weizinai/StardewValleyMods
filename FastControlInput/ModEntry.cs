using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.FastControlInput.Framework;
using weizinai.StardewValleyMod.FastControlInput.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.FastControlInput;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private IInputHandler[] handlers = Array.Empty<IInputHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForFastControlInput(),
            () => this.config,
            value => this.config = value,
            this.UpdateConfig,
            this.UpdateConfig
        );
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        foreach (var handler in this.handlers)
        {
            if (handler.IsEnable())
            {
                handler.Update();
            }
        }
    }

    private void UpdateConfig()
    {
        this.handlers = this.GetHandlers().ToArray();
    }

    private IEnumerable<IInputHandler> GetHandlers()
    {
        if (this.config.ActionButton > 1) yield return new ActionButtonHandler(this.config.ActionButton);
        if (this.config.UseToolButton > 1) yield return new UseToolButtonHandler(this.config.UseToolButton);
    }
}