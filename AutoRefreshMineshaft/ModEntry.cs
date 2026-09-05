using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.AutoRefreshMineShaft.Framework;
using weizinai.StardewValleyMod.PiCore.Integration.GenericModConfigMenu;

namespace weizinai.StardewValleyMod.AutoRefreshMineShaft;

internal class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        this.config = helper.ReadConfig<ModConfig>();
        // 注册事件
        this.Helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Helper.Events.Player.Warped += this.OnWarped;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.AddGenericModConfigMenu(
            new GenericModConfigMenuIntegrationForAutoRefreshMineShaft(),
            () => this.config,
            value => this.config = value
        );
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (!this.config.EnableMod) return;

        if (e is { OldLocation: MineShaft, NewLocation: not MineShaft })
        {
            if (Game1.IsServer)
            {
                this.RefreshMineshaft();
            }
            else
            {
                this.Helper.Multiplayer.SendMessage(
                    "",
                    "RefreshMineshaft",
                    new[] { "weizinai.AutoRefreshMineshaft" },
                    new[] { Game1.MasterPlayer.UniqueMultiplayerID }
                );
            }
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.Type == "RefreshMineshaft")
        {
            this.RefreshMineshaft();
        }
    }

    private void RefreshMineshaft()
    {
        MineShaft.activeMines.RemoveAll(mine =>
        {
            if (mine.mineLevel <= 120 && !mine.farmers.Any())
            {
                mine.OnRemoved();
                return true;
            }
            return false;
        });
    }
}