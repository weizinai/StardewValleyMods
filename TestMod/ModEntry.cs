using Common;
using Common.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using TestMod.Framework;
using TestMod.Patches;

namespace TestMod;

public class ModEntry : Mod
{
    private ModConfig config = null!;

    public override void Entry(IModHelper helper)
    {
        // 初始化
        config = helper.ReadConfig<ModConfig>();
        // 注册事件
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        // 注册Harmony补丁
        HarmonyPatcher.Patch(this,new MineShaftPatcher(config), new VolcanoDungeonPatcher(config));
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

        configMenu.AddNumberOption(
            ModManifest,
            () => config.MineShaftMap,
            value => config.MineShaftMap = value,
            () => "矿井新地图",
            null,
            40,
            60
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => config.VolcanoDungeonMap,
            value => config.VolcanoDungeonMap = value,
            () => "火山新地图",
            null,
            38,
            57
        );
    }
}