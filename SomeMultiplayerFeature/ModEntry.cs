using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.Common.Handler;
using weizinai.StardewValleyMod.Common.Integration;
using weizinai.StardewValleyMod.Common.Log;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Framework;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Handler;
using weizinai.StardewValleyMod.SomeMultiplayerFeature.Patcher;

namespace weizinai.StardewValleyMod.SomeMultiplayerFeature;

public class ModEntry : Mod
{
    public const string ModDataPrefix = "weizinai.SMF.";
    public const string ModUniqueId = "weizinai.SomeMultiplayerFeature";

    private ModConfig config = null!;
    private IHandler[] handlers = Array.Empty<IHandler>();

    public static ModEntry Instance { get; private set; } = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        MultiplayerLog.Init(this);
        this.config = helper.ReadConfig<ModConfig>();
        Instance = this;
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
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
            new GameLocationPatcher(this.Helper),
            new HoeDirtPatcher(),
            new MineShaftPatcher(),
            new MuseumMenuPatcher(),
            new ShopMenuPatcher(),
            new TreePatcher()
        );
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();

        this.handlers = new IHandler[]
        {
            new AutoClickHandler(this.Helper, this.config),
            new SecretarySystemHandler(this.Helper, this.config),
            new CustomCommandHandler(this.Helper, this.config),
            new DataHandler(this.Helper),
            new IpConnectionHandler(this.Helper, this.config),
            new MineshaftHandler(this.Helper),
            new PerfectFishingHandler(this.Helper),
            new PlayerCountHandler(this.Helper, this.config),
            new SpendLimitHandler(this.Helper, this.config),
            new TipHandler(this.Helper, this.config),
            new UnreadyPlayerHandler(this.Helper, this.config),
            new VersionLimitHandler(this.Helper, this.config)
        };

        foreach (var handler in this.handlers) handler.Apply();
    }
}