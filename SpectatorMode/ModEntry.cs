using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    public static ModConfig Config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        Config = helper.ReadConfig<ModConfig>();
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (Game1.timeOfDay == 2600 && Game1.activeClickableMenu is SpectatorMenu menu) menu.exitThisMenu();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForSpectatorMode(
            this.Helper,
            this.ModManifest,
            () => Config,
            () => Config = new ModConfig(),
            () => this.Helper.WriteConfig(Config)
        ).Register();
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new CommandHandler(this.Helper),
            new SpectateLocationHandler(this.Helper),
            new SpectatePlayerHandler(this.Helper)
        };

        foreach (var handler in handlers) handler.Init();
    }
}