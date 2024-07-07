using SaveModInfo.Framework;
using SaveModInfo.Handler;
using SaveModInfo.Patcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace SaveModInfo;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        this.InitHandler();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new LoadGameMenuPatcher(), new SaveFileSlotPatcher());
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForSaveGameInfo(
            new GenericModConfigMenuIntegration<ModConfig>(
                this.Helper.ModRegistry,
                this.ModManifest,
                () => this.config,
                () =>
                {
                    this.config = new ModConfig();
                    this.Helper.WriteConfig(this.config);
                },
                () => this.Helper.WriteConfig(this.config)
            )
        ).Register();
    }

    private void InitHandler()
    {
        var handlers = new IHandler[]
        {
            new RecordModInfoHandler(this.Helper),
            new CheckModInfoHandler(this.Helper)
        };
        
        foreach (var handler in handlers) handler.Init();
    }
}