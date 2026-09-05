using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        ModConfig.Init(helper);
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuForSpectatorMode(),
            () => ModConfig.Instance,
            config => ModConfig.Instance = config
        );
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new AutoEventHandler(this.Helper),
            new AutoFestivalHandler(this.Helper),
            new AutoSleepHandler(this.Helper),
            new CommandHandler(this.Helper),
            new SpectateLocationHandler(this.Helper),
            new SpectatePlayerHandler(this.Helper)
        };

        foreach (var handler in handlers) handler.Apply();
    }
}