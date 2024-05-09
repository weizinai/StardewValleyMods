using Common.Integrations;
using Common.Patch;
using FriendshipDecayModify.Framework;
using FriendshipDecayModify.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FriendshipDecayModify;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        I18n.Init(helper.Translation);
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this, new GameLocationPatcher(config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        configMenu.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));

        configMenu.AddNumberOption(
            ModManifest,
            () => config.GarbageCanModify,
            value => config.GarbageCanModify = value,
            I18n.Config_GarbageCanModify_Name,
            null,
            0,
            50,
            5
        );
    }
}