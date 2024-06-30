using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.SpectatorMode.Framework;
using weizinai.StardewValleyMod.SpectatorMode.Handler;

namespace weizinai.StardewValleyMod.SpectatorMode;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    
    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        SpectatorHelper.Init(this.config);
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new CommandHandler(this.Helper, this.config),
            new RotatePlayerHandler(this.Helper, this.config),
            new SpectateLocationHandler(this.Helper, this.config),
            new SpectatePlayerHandler(this.Helper, this.config)
        };

        foreach (var handler in handlers) handler.Init();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuForSpectatorMode(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () => this.Helper.WriteConfig(this.config)
        ).Register();
    }
}