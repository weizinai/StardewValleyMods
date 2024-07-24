using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.FastControlInput.Framework;
using weizinai.StardewValleyMod.FastControlInput.Handler;

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
        new GenericModConfigMenuIntegrationForFastControlInput(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => this.config,
                () =>
                {
                    this.config = new ModConfig();
                    this.Helper.WriteConfig(this.config);
                    this.UpdateConfig();
                },
                () =>
                {
                    this.Helper.WriteConfig(this.config);
                    this.UpdateConfig();
                }
            )
        ).Register();
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