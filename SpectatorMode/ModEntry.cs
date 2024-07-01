using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    public ModConfig Config = null!;
    public static ModEntry Instance { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.Config = helper.ReadConfig<ModConfig>();
        Instance = this;
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
            () => this.Config,
            () => this.Config = new ModConfig(),
            () => this.Helper.WriteConfig(this.Config)
        ).Register();
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new CommandHandler(),
            new RotatePlayerHandler(),
            new SpectateLocationHandler(),
            new SpectatePlayerHandler()
        };

        foreach (var handler in handlers) handler.Init();
    }
}