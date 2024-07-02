using weizinai.StardewValleyMod.Common.Log;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using weizinai.StardewValleyMod.LazyMod.Framework;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using weizinai.StardewValleyMod.LazyMod.Framework.Hud;
using weizinai.StardewValleyMod.LazyMod.Framework.Integration;
using weizinai.StardewValleyMod.LazyMod.Handler;

namespace weizinai.StardewValleyMod.LazyMod;

internal class ModEntry : Mod
{
    private ModConfig config = null!;
    private MiningHud miningHud = null!;
    private IAutomationHandler[] handlers = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        Log.Init(this.Monitor);
        I18n.Init(helper.Translation);
        try
        {
            this.config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception)
        {
            helper.WriteConfig(new ModConfig());
            this.config = helper.ReadConfig<ModConfig>();
            Log.Info("Read config.json file failed and was automatically fixed. Please reset the features you want to turn on.");
        }

        this.UpdateConfig();

        // 注册事件
        helper.Events.Display.RenderedHud += this.OnRenderedHud;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree) return;

        var player = Game1.player;
        var location = Game1.currentLocation;
        if (player is null || location is null) return;
        
        TileHelper.ClearTileCache();
        foreach (var handler in this.handlers) handler.Apply(player, location);

        this.miningHud.Update();
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.miningHud.Draw(e.SpriteBatch);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.miningHud = new MiningHud(this.Helper, this.config);

        new GenericModConfigMenuIntegrationForLazyMod(
            this.Helper,
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
        ).Register();
    }

    private void UpdateConfig()
    {
        this.handlers = this.GetHandlers().ToArray();
    }

    private IEnumerable<IAutomationHandler> GetHandlers()
    {
        // Animal
        if (this.config.AutoPetAnimal.IsEnable) yield return new PetAnimalHandler(this.config);
        if (this.config.AutoPetPet.IsEnable) yield return new PetPetHandler(this.config);
    }
}