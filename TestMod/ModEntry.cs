using Common;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using TestMod.Framework;

namespace TestMod;

public class ModEntry : Mod
{
    public static ModConfig Config = new();

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<ModConfig>();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        var harmony = new Harmony("weizinai.TestMod");
        harmony.Patch(
            AccessTools.Method(typeof(MineShaft), nameof(MineShaft.loadLevel), new[] { typeof(int) }),
            transpiler: new HarmonyMethod(typeof(MineShaftPatch), nameof(MineShaftPatch.LoadLevelTranspiler))
        );
        harmony.Patch(
            AccessTools.Method(typeof(VolcanoDungeon), nameof(VolcanoDungeon.GenerateLevel), new[] { typeof(bool) }),
            prefix: new HarmonyMethod(typeof(VolcanoDungeonPatch), nameof(VolcanoDungeonPatch.GenerateLevelPrefix))
        );
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

        if (configMenu is null) return;

        configMenu.Register(
            ModManifest,
            () => Config = new ModConfig(),
            () => Helper.WriteConfig(Config)
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => Config.MapNumberToLoad,
            value => Config.MapNumberToLoad = value,
            () => "矿井新地图",
            null,
            40,
            60
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => Config.LayoutIndex,
            value => Config.LayoutIndex = value,
            () => "火山新地图",
            null,
            38,
            57
        );
    }
}