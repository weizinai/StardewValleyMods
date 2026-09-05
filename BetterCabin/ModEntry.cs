using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using weizinai.StardewValleyMod.BetterCabin.Framework;
using weizinai.StardewValleyMod.BetterCabin.Framework.Config;
using weizinai.StardewValleyMod.BetterCabin.Handler;
using weizinai.StardewValleyMod.BetterCabin.Patcher;
using weizinai.StardewValleyMod.Common;
using weizinai.StardewValleyMod.PiCore.Handler;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;
using weizinai.StardewValleyMod.PiCore.Patcher;

namespace weizinai.StardewValleyMod.BetterCabin;

internal class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.BetterCabin.";

    private readonly List<IHandler> handlers = new();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        Logger.Init(this.Monitor);
        ModConfig.Init(helper);
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Apply(this.ModManifest.UniqueID,
            new BuildingPatcher(),
            new CarpenterMenuPatcher(),
            new ForceBuildCabinPatcher(),
            new Game1Patcher(),
            new PassableMailboxPatcher()
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForBetterCabin(),
            () => ModConfig.Instance,
            config => ModConfig.Instance = config,
            this.UpdateConfig,
            this.UpdateConfig
        );
    }

    private void UpdateConfig()
    {
        var config = ModConfig.Instance;

        foreach (var handler in this.handlers) handler.Clear();
        this.handlers.Clear();

        if (config.ResetCabinPlayer)
            this.handlers.Add(new ResetCabinHandler(this.Helper));
        if (config.CabinMenu)
            this.handlers.Add(new CabinMenuHandler(this.Helper));
        if (config.VisitCabinInfo)
            this.handlers.Add(new VisitCabinInfoHandler(this.Helper));
        if (config.LockCabin)
            this.handlers.Add(new LockCabinHandler(this.Helper));

        this.handlers.Add(new CabinCostHandler(this.Helper));

        foreach (var handler in this.handlers) handler.Apply();
    }
}