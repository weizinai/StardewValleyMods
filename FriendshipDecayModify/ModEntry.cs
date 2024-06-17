using Common.Patcher;
using FriendshipDecayModify.Framework;
using FriendshipDecayModify.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FriendshipDecayModify;

internal class ModEntry : Mod
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
        HarmonyPatcher.Apply(this, new GameLocationPatcher(config), new FarmerPatcher(config), new NPCPatcher(config), new FarmAnimalPatcher(config));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        new GenericModConfigMenuIntegrationForFriendshipDecayModify(
            Helper,
            ModManifest,
            () => config,
            () => config = new ModConfig(),
            () => Helper.WriteConfig(config)
        ).Register();
    }
}