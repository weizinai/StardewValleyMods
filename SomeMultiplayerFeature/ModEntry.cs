using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handlers;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

public class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.SMF.";

    private ModConfig config = null!;
    private IHandler[] handlers = Array.Empty<IHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        MultiplayerLog.Init(this);
        this.config = helper.ReadConfig<ModConfig>();
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // 注册GenericModConfigMenu
        new GenericModConfigMenuIntegrationForSomeMultiplayerFeature(
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
                }),
            this.Helper.Events.Input
        ).Register();

        // 注册Harmony补丁
        HarmonyPatcher.Apply(this,
            new FarmAnimalPatcher(),
            new FarmerPatcher(),
            new FarmHousePatcher(),
            new Game1Patcher(),
            new GameLocationPatcher(),
            new HoeDirtPatcher(),
            new MineShaftPatcher(),
            new ShopMenuPatcher(),
            new TreePatcher()
        );
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        var modData = Game1.player.modData;
        var keysToRemove = modData.Keys.Where(key => key.StartsWith("weizinai.Some")).ToList();

        if (keysToRemove.Count > 0)
        {
            Log.NoIconHUDMessage("检测到无用的旧版本数据，已清除。");
            foreach (var key in keysToRemove)
            {
                modData.Remove(key);
            }
        }
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();

        this.handlers = new IHandler[]
        {
            new AutoClickHandler(this.Helper, this.config),
            new CustomCommandHandler(this.Helper, this.config),
            new DataHandler(this.Helper),
            new IpConnectionHandler(this.Helper, this.config),
            new MineshaftHandler(this.Helper),
            new PlayerCountHandler(this.Helper, this.config),
            new SpendLimitHandler(this.Helper, this.config),
            new TipHandler(this.Helper, this.config),
            new UnreadyPlayerHandler(this.Helper, this.config),
            new VersionLimitHandler(this.Helper, this.config)
        };

        foreach (var handler in this.handlers) handler.Apply();
    }
}