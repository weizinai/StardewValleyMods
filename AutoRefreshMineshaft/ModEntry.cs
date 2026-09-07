using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using weizinai.StardewValleyMod.AutoRefreshMineShaft.Framework;
using weizinai.StardewValleyMod.PiCore.Config;

namespace weizinai.StardewValleyMod.AutoRefreshMineShaft;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        // 初始化
        I18n.Init(helper.Translation);
        // 配置模块接管读取（损坏自愈）、GMCM 生命周期与保存/重置，读到的实例写入静态 ModConfig.Instance 供事件处理器读取
        var configService = new ConfigService<ModConfig>(
            this,
            () => ModConfig.Instance,
            value => ModConfig.Instance = value
        );
        configService.RegisterMenu(this.BuildConfigMenu);
        // 注册事件
        this.Helper.Events.Player.Warped += this.OnWarped;
        this.Helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (!ModConfig.Instance.EnableMod)
        {
            return;
        }

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

    /// <summary>用声明式描述器声明本模组的 GMCM 配置菜单，由配置模块渲染。</summary>
    /// <param name="menu">配置菜单描述器。</param>
    private void BuildConfigMenu(ConfigMenuDescriptor<ModConfig> menu)
    {
        menu.AddBoolOption(config => config.EnableMod, I18n.Config_EnableMod_Name);
    }
}
