using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.BetterCabin.Patcher;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin;

internal class ModEntry : Mod
{
    private readonly List<IHandler> handlers = new();
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this,
            new BuildingPatcher(this.config),
            new CarpenterMenuPatcher(this.config),
            new Game1Patcher(),
            new PassableMailboxPatcher(this.config)
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForBetterCabin(
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

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();
        this.handlers.Clear();

        if (this.config.ResetCabinPlayer)
            this.handlers.Add(new ResetCabinHandler(this.Helper, this.config));
        if (this.config.CabinMenu)
            this.handlers.Add(new CabinMenuHandler(this.Helper, this.config));
        if (this.config.VisitCabinInfo)
            this.handlers.Add(new VisitCabinInfoHandler(this.Helper, this.config));
        if (this.config.LockCabin)
            this.handlers.Add(new LockCabinHandler(this.Helper, this.config));

        this.handlers.Add(new CabinCostHandler(this.Helper, this.config));

        foreach (var handler in this.handlers) handler.Apply();
    }
}