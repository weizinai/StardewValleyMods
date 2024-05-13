using Common.Integrations;
using Common.Patch;
using SomeMultiplayerFeature.Framework;
using SomeMultiplayerFeature.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SomeMultiplayerFeature;

public class ModEntry : Mod
{
    private ModConfig config = new();
    private IClickableMenu? lastShopMenu;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Input.ButtonsChanged += OnButtonChanged;
        helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        // 注册Harmony补丁
        // HarmonyPatcher.Apply(this, new UtilityPatcher(helper), new IClickableMenuPatcher(helper));
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.activeClickableMenu is ShopMenu && lastShopMenu is not ShopMenu)
        {
            Game1.addHUDMessage(new HUDMessage("进入商店"));
        }
        else if (lastShopMenu is ShopMenu && Game1.activeClickableMenu is not ShopMenu)
        {
            Game1.addHUDMessage(new HUDMessage("离开商店"));
        }
        
        lastShopMenu = Game1.activeClickableMenu;
    }

    private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsMultiplayer) return;

        if (config.ShowModInfoKeybind.JustPressed())
        {
            var modInstall = true;
            foreach (var farmer in Game1.getOnlineFarmers())
            {
                var peer = Helper.Multiplayer.GetConnectedPlayer(farmer.UniqueMultiplayerID);
                if (peer is null) continue;
                if (peer.GetMod(ModManifest.UniqueID) is null)
                {
                    var hudMessage = new HUDMessage($"{farmer.Name}没有安装该模组")
                    {
                        noIcon = true,
                        timeLeft = 500f,
                    };
                    Game1.addHUDMessage(hudMessage);
                    modInstall = false;
                }
            }

            if (modInstall)
            {
                var hudMessage = new HUDMessage("所有人都安装了模组")
                {
                    noIcon = true,
                    timeLeft = 500f,
                };
                Game1.addHUDMessage(hudMessage);
            }
        }

        if (Context.IsMainPlayer && Game1.activeClickableMenu is ReadyCheckDialog menu && config.SetAllPlayerReadyKeybind.JustPressed())
        {
            Helper.Multiplayer.SendMessage(menu.checkName, "SetAllPlayerReady", new[] { "weizinai.SomeMultiplayerFeature" });
        }
    }

    private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
    {
        if (config.ShowShopInfo && e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "ShopMessage" })
        {
            var message = e.ReadAs<Message>();
            var hudMessage = new HUDMessage(message.ToString())
            {
                noIcon = true,
                timeLeft = 500f,
                type = message.PlayerName + message.IsExit
            };
            Game1.addHUDMessage(hudMessage);
        }

        if (e is { FromModID: "weizinai.SomeMultiplayerFeature", Type: "SetAllPlayerReady" })
        {
            var message = e.ReadAs<string>();
            switch (message)
            {
                case "sleep":
                    Helper.Reflection.GetMethod(new GameLocation(), "startSleep").Invoke();
                    break;
                default:
                    Game1.netReady.SetLocalReady(message, true);
                    break;
            }
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        );

        configMenu.AddBoolOption(
            ModManifest,
            () => config.ShowShopInfo,
            value => config.ShowShopInfo = value,
            I18n.Config_ShowShopInfo_Name
        );
        configMenu.AddKeybindList(
            ModManifest,
            () => config.ShowModInfoKeybind,
            value => config.ShowModInfoKeybind = value,
            I18n.Config_ShowModInfoKeybind_Name
        );
    }
}