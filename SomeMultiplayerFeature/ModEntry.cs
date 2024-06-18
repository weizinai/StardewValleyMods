using Common;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SomeMultiplayerFeature;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(Monitor);
        I18n.Init(helper.Translation);
        config = helper.ReadConfig<ModConfig>();
        InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForSomeMultiplayerFeature(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new AccessShopInfoHandler(Helper, config),
            new DelayedPlayerHandler(Helper, config),
            new ModLimitHandler(Helper, config),
            // new MoneyCheatHandler(Helper, config),
            new PlayerCountHandler(Helper, config),
            new TipHandler(Helper, config),
            new UnreadyPlayerHandler(Helper, config)
        };

        foreach (var handler in handlers) handler.Init();
    }
}