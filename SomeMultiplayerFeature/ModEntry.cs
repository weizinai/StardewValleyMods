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
    private ModConfig config = null!;
    private IHandler[] handlers = Array.Empty<IHandler>();

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        new CustomCommandHandler(helper, this.config).Apply();
        this.UpdateConfig();
        // 注册事件
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
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
    }

    private void UpdateConfig()
    {
        foreach (var handler in this.handlers) handler.Clear();

        this.handlers = new IHandler[]
        {
            new AutoClickHandler(this.Helper, this.config),
            new CabinCostHandler(this.Helper),
            new FreezeMoneyHandler(this.Helper, this.config),
            new HouseUpgradeHandler(this.Helper, this.config),
            new IpConnectionHandler(this.Helper, this.config),
            new MachineExperienceHandler(this.Helper),
            new PlayerCountHandler(this.Helper, this.config),
            new PurchaseBackpackHandler(this.Helper, this.config),
            new TipHandler(this.Helper, this.config),
            new UnreadyPlayerHandler(this.Helper, this.config),
            new VersionLimitHandler(this.Helper, this.config)
        };

        foreach (var handler in this.handlers) handler.Apply();
    }
}