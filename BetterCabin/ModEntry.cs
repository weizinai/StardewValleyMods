using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.BetterCabin.Patcher;
using weizinai.StardewValleyMod.Common.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin;

internal class ModEntry : Mod
{
    private readonly List<IHandler> handlers = new();
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        this.config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this, new BuildingPatcher(this.config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForBetterCabin(this.Helper, this.ModManifest,
            () => this.config,
            () => this.config = new ModConfig(),
            () =>
            {
                this.Helper.WriteConfig(this.config);
                this.UpdateConfig();
            }
        ).Register();
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();
        this.handlers.Clear();

        if (this.config.ResetCabinPlayer)
            this.handlers.Add(new ResetCabinHandler(this.config, this.Helper));
        if (this.config.SetCabinSkin)
            this.handlers.Add(new SetCabinSkinHandler(this.config, this.Helper));
        if (this.config.VisitCabinInfo)
            this.handlers.Add(new VisitCabinInfoHandler(this.config, this.Helper));

        foreach (var handler in this.handlers) handler.Init();
    }
}